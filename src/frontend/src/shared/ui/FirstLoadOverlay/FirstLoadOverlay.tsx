import { useEffect, useState, useRef } from 'react';
import { gsap } from 'gsap';
import styles from './FirstLoadOverlay.module.css';

export const FirstLoadOverlay = () => {
  const [show, setShow] = useState(true);
  const overlayRef = useRef<HTMLDivElement>(null);
  const logoRef = useRef<HTMLDivElement>(null);
  
  useEffect(() => {
    const tl = gsap.timeline({
      onComplete: () => {
        setShow(false);
      }
    });
    
    tl.set(logoRef.current, {
      opacity: 0,
      scale: 0.8,
      y: 20
    });
    
    tl.to(logoRef.current, {
      opacity: 1,
      scale: 1,
      y: 0,
      duration: 0.8,
      ease: "power3.out"
    });
    
    tl.to({}, { duration: 0.6 });
    
    tl.to(logoRef.current, {
      opacity: 0,
      scale: 1.2,
      duration: 0.5,
      ease: "power2.in"
    }, "fadeOut");
    
    tl.to(overlayRef.current, {
      opacity: 0,
      duration: 0.8,
      ease: "power2.inOut"
    }, "fadeOut");
    
    return () => {
      tl.kill();
    };
  }, []);

  if (!show) return null;

  return (
    <div ref={overlayRef} className={styles.overlay}>
      <div ref={logoRef} className={styles.logoContainer}>
        <div className={styles.logo}>MikuLearn</div>
        <div className={styles.loadingBar}>
          <div className={styles.loadingProgress}></div>
        </div>
      </div>
    </div>
  );
}; 