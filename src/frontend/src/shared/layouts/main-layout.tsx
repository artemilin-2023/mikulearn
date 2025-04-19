import { ReactNode } from 'react';
import { Header } from '@shared/ui';

interface PageProps {
  children: ReactNode;
}

export const MainLayout = ({ children }: PageProps) => {
  return (
    <>
      <Header />
      <main>{children}</main>
    </>
  );
};
