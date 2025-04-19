import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "./providers/Router.provider";
import { MantineProvider, createTheme } from "@mantine/core";


// global styles
import '@mantine/core/styles.css';
import "./styles/layout.css";
import "./styles/colors.css";
import "./styles/reset.css";
import "./styles/transitions.css";




const theme = createTheme({
  primaryColor: "blue",
  fontFamily: "Inter, sans-serif",
  defaultRadius: "md",
});



createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <MantineProvider 
      defaultColorScheme="light"
      theme={theme}
    >
      <RouterProvider />
    </MantineProvider>
  </StrictMode>,
);
