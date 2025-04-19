import { ReactNode } from "react";
import { Header } from "widgets/Header/Header"; 

interface PageProps {
  children: ReactNode;
}

export const Page = ({ children }: PageProps) => {
  return (
    <>
      <Header />
      <main>
        {children}
      </main>
    </>
  );
};