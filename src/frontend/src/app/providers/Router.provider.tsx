import { Suspense, useRef, useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route, useLocation } from 'react-router-dom';
import { Page } from 'shared/layouts/Page';

// pages
import { IndexPage } from 'pages/Index/IndexPage';
import { FallbackPage } from 'pages/Fallback/FallbackPage';
import { NotFoundPage } from 'pages/NotFound/NotFoundPage';

import { CSSTransition, SwitchTransition } from "react-transition-group";
import { Power3, Power4, gsap } from "gsap";
import { FirstLoadOverlay } from 'shared/ui/FirstLoadOverlay/FirstLoadOverlay';

const AnimatedRoutes = () => {
  const location = useLocation();
  const nodeRef = useRef(null);
  const isFirstLoad = !localStorage.getItem('hasVisitedBefore');

  useEffect(() => {
    if (isFirstLoad) {
      localStorage.setItem('hasVisitedBefore', 'true');
    }
  }, [isFirstLoad]);

  const onEnter = (node: HTMLElement) => {
    if (!node) return;
    
    if (isFirstLoad) {
      const children = Array.from(node.children);
      
      gsap.set(children, {
        y: 50,
        opacity: 0,
        scale: 0.9
      });
      
      const mainElements = document.querySelectorAll('.logo, h1, .title');
      if (mainElements.length > 0) {
        gsap.fromTo(
          mainElements,
          {
            opacity: 0,
            scale: 0.8,
            y: -30
          },
          {
            opacity: 1,
            scale: 1,
            y: 0,
            duration: 1.2,
            ease: Power4.easeOut,
            delay: 0.3
          }
        );
      }
      
      gsap.to(children, {
        y: 0,
        opacity: 1,
        scale: 1,
        duration: 0.8,
        delay: 0.5,
        ease: Power3.easeOut,
        stagger: {
          amount: 0.6,
          from: "start"
        }
      });
      
      const gridItems = document.querySelectorAll('.gridItem');
      if (gridItems.length > 0) {
        gsap.fromTo(
          gridItems,
          {
            opacity: 0,
            y: 30,
            scale: 0.8
          },
          {
            opacity: 1,
            y: 0,
            scale: 1,
            duration: 0.6,
            delay: 0.8,
            ease: Power3.easeOut,
            stagger: {
              amount: 0.5,
              from: "center",
              grid: "auto"
            }
          }
        );
      }
    } else {
      gsap.fromTo(
        node.children,
        {
          y: 30,
          opacity: 0
        },
        {
          y: 0,
          opacity: 1,
          duration: 0.1,
          delay: 0.1,
          ease: Power3.easeIn,
          stagger: {
            amount: 0.2
          }
        }
      );
    }
  };

  const onExit = (node: HTMLElement) => {
    if (!node) return;
    
    gsap.to(
      node.children,
      {
        y: -30,
        opacity: 0,
        duration: 0.2,
        ease: Power3.easeIn,
        stagger: {
          amount: 0.1
        }
      }
    );
  };

  return (
    <SwitchTransition mode="out-in">
      <CSSTransition
        key={location.pathname}
        nodeRef={nodeRef}
        timeout={800}
        classNames="page"
        onExit={(node: HTMLElement) => onExit(node)}
        onEntering={(node: HTMLElement) => onEnter(node)}
        unmountOnExit
      >
        <div ref={nodeRef} className="page-transition-container">
          <Routes location={location}>
            <Route path="/" element={<IndexPage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </div>
      </CSSTransition>
    </SwitchTransition>
  );
};

export const RouterProvider = () => {
  const [showOverlay, setShowOverlay] = useState(!localStorage.getItem('hasVisitedBefore'));
  
  useEffect(() => {
    if (showOverlay) {
      localStorage.setItem('hasVisitedBefore', 'true');
      
      const timer = setTimeout(() => {
        setShowOverlay(false);
      }, 4000);
      
      return () => clearTimeout(timer);
    }
  }, [showOverlay]);
  
  return (
    <BrowserRouter>
      {showOverlay && <FirstLoadOverlay />}
      <Suspense fallback={<FallbackPage />}>
        <Page>
            <AnimatedRoutes />
        </Page>
      </Suspense>
    </BrowserRouter>
  );
};
