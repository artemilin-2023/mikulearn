import { BrowserRouter, Routes, Route, Outlet } from 'react-router-dom';
import { MainLayout } from '@shared/layouts';
import { HomePage } from '@pages/home/home-page';
import { DashboardPage } from '@pages/dashboard/dashboard-page';
import { SignInPage } from '@pages/sign-in/sign-in-page';
import { SignUpPage } from '@pages/sign-up/sign-up-page';
import { PersonalCabinetPage } from '@pages/personal-cabinet/personal-cabinet-page';
import { NotFoundPage } from '@pages/not-found/not-found-page';

const Layout = () => {
  return (
    <MainLayout>
      <Outlet />
    </MainLayout>
  );
};

export const RouterProvider = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/sign-in" element={<SignInPage />} />
          <Route path="/sign-up" element={<SignUpPage />} />
          <Route path="/personal-cabinet" element={<PersonalCabinetPage />} />
        </Route>
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
};
