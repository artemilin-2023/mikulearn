import { Loader } from '@mantine/core';
import {
  useContext,
  useEffect,
  useState,
} from 'react';

import {
  BrowserRouter,
  Routes,
  Route,
  Outlet,
  Navigate,
} from 'react-router-dom';

import { HomePage } from '@pages/home/home-page';
import { NotFoundPage } from '@pages/not-found/not-found-page';
import { PersonalCabinetPage } from '@pages/personal-cabinet/personal-cabinet-page';
import { SignInPage } from '@pages/sign-in/sign-in-page';
import { SignUpPage } from '@pages/sign-up/sign-up-page';
import { TestPage } from '@pages/test/test-page';
import { MainLayout } from '@shared/layouts';
import AuthService, { User } from '@shared/services/AuthService/AuthService';
import { StoreContext } from '@shared/store/store';


const Layout = () => {
  return (
    <MainLayout>
      <Outlet />
    </MainLayout>
  );
};

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const store = useContext(StoreContext);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  
  useEffect(() => {
    const checkAuth = async () => {
      if (store.isAuth) {
        setIsAuthenticated(true);
        setIsLoading(false);
        return;
      }
      
      if (store.token) {
        try {
          const response = await AuthService.getMe();
          if (response.data) {
            const responseData = response.data as unknown as User;
            const userData: User = {
              id: responseData.id,
              email: responseData.email,
              name: responseData.name || '',
              roles: responseData.roles,
            };
            store.setUser(userData);
            store.setIsAuth(true);
            setIsAuthenticated(true);
          }
        } catch (error) {
          console.error('Failed to authenticate with token:', error);
          setIsAuthenticated(false);
        }
      } else {
        setIsAuthenticated(false);
      }
      
      setIsLoading(false);
    };

    if (store.initialized) {
      checkAuth();
    } else {
      const checkInterval = setInterval(() => {
        if (store.initialized) {
          clearInterval(checkInterval);
          checkAuth();
        }
      }, 100);
      
      return () => clearInterval(checkInterval);
    }
  }, [store]);
  
  if (isLoading) {
    return (
      <div style={{
        display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', 
      }}>
        <Loader size="xl" color="primary" />
      </div>
    );
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/sign-in" replace />;
  }
  
  return <>{children}</>;
};

const GuestOnlyRoute = ({ children }: { children: React.ReactNode }) => {
  const store = useContext(StoreContext);
  const [isLoading, setIsLoading] = useState(true);
  
  useEffect(() => {
    if (store.initialized) {
      setIsLoading(false);
    } else {
      const checkInterval = setInterval(() => {
        if (store.initialized) {
          clearInterval(checkInterval);
          setIsLoading(false);
        }
      }, 100);
      
      return () => clearInterval(checkInterval);
    }
  }, [store]);
  
  if (isLoading) {
    return (
      <div style={{
        display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', 
      }}>
        <Loader size="xl" color="primary" />
      </div>
    );
  }
  
  if (store.isAuth) {
    return <Navigate to="/personal-cabinet" replace />;
  }
  
  return <>{children}</>;
};

export const RouterProvider = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route path="/" element={<HomePage />} />
          
          {/* Guest-only routes */}
          <Route path="/sign-in" element={
            <GuestOnlyRoute>
              <SignInPage />
            </GuestOnlyRoute>
          } />
          <Route path="/sign-up" element={
            <GuestOnlyRoute>
              <SignUpPage />
            </GuestOnlyRoute>
          } />
          
          {/* Protected routes */}
          <Route path="/personal-cabinet" element={
            <ProtectedRoute>
              <PersonalCabinetPage />
            </ProtectedRoute>
          } />
          <Route path="/test" element={
            <ProtectedRoute>
              <TestPage />
            </ProtectedRoute>
          } />
          
        </Route>
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
};
