import { useEffect, useState, useRef } from 'react';
import { gsap } from 'gsap';
import styles from './FirstLoadOverlay.module.css';

export const FirstLoadOverlay = () => {
  const [show, setShow] = useState(true);
  const particlesRef = useRef<HTMLDivElement>(null);
  const overlayRef = useRef<HTMLDivElement>(null);
  const contentRef = useRef<HTMLDivElement>(null);
  
  // делаем частицы
  useEffect(() => {
    if (!particlesRef.current) return;
    
    const particles = particlesRef.current;
    const particleCount = 12;
    
    particles.innerHTML = '';
    // убираем частицы
    
    // новые частицы
    for (let i = 0; i < particleCount; i++) {
      const particle = document.createElement('div');
      particle.className = styles.particle;
      
      // рандом размер
      const size = Math.random() * 30 + 10;
      particle.style.width = `${size}px`;
      particle.style.height = `${size}px`;
      
      // рандом позиция
      particle.style.left = `${Math.random() * 100}%`;
      particle.style.top = `${Math.random() * 100}%`;
      
      // рандом прозрачность
      particle.style.opacity = `${Math.random() * 0.2}`;
      
      particles.appendChild(particle);
    }
  }, []);

  useEffect(() => {
    const mainTl = gsap.timeline({
      onComplete: () => {
        // хуйня
        const exitTl = gsap.timeline({
          onComplete: () => setShow(false)
        });
        
        // хуйня
        if (particlesRef.current) {
          const particles = particlesRef.current.children;
          
          // хуйня
          exitTl.to(particles, {
            opacity: 0.2,
            scale: 0.8,
            filter: "blur(8px)",
            duration: 0.8,
            ease: "power2.in",
            stagger: 0.03
          }, 0);
        }
        
        // эффект размытия и уменьшения прозрачности для всего оверлея
        exitTl.to(overlayRef.current, {
          opacity: 0.7,
          filter: "blur(5px)",
          duration: 0.6,
          ease: "power2.in"
        }, 0.1);
        
        // исчезновение
        exitTl.to(overlayRef.current, {
          opacity: 0,
          filter: "blur(20px)",
          duration: 0.8,
          ease: "power3.inOut",
          onStart: () => {
            // вспышка перед исчезновением (ГОВНО ЕБАНОЕ БЛЯТЬ)
            const flash = document.createElement('div');
            flash.className = styles.flash;
            document.body.appendChild(flash);
            
            gsap.fromTo(flash, 
              { opacity: 0 },
              { 
                opacity: 0.2, 
                duration: 0.3,
                onComplete: () => {
                  gsap.to(flash, {
                    opacity: 0,
                    duration: 0.6,
                    onComplete: () => {
                      document.body.removeChild(flash);
                    }
                  });
                }
              }
            );
          }
        }, 0.4);
      }
    });

    // анимация частиц (небольшой эффект фона)
    if (particlesRef.current) {
      const particles = particlesRef.current.children;
      gsap.set(particles, { scale: 0, opacity: 0 });
      
      mainTl.to(particles, {
        scale: 1,
        opacity: () => Math.random() * 0.3,
        duration: 1.5,
        stagger: 0.1,
        ease: "power3.out"
      });
      
      // летающие частицы
      gsap.to(particles, {
        y: "random(-30, 30)",
        x: "random(-20, 20)",
        rotation: "random(-15, 15)",
        duration: "random(3, 6)",
        repeat: -1,
        yoyo: true,
        ease: "sine.inOut",
        stagger: 0.1
      });
    }

    // анимация лого
    mainTl.fromTo(
      `.${styles.logo}`,
      { y: 30, opacity: 0 },
      { y: 0, opacity: 1, duration: 1, ease: "power3.out" },
      0.5
    )
    .fromTo(
      `.${styles.logoHighlight}`,
      { scaleX: 0, opacity: 0 },
      { scaleX: 1, opacity: 0.3, duration: 0.8, ease: "power2.out" },
      "-=0.4"
    );

    // прогресс бар
    mainTl.fromTo(
      `.${styles.progressBarContainer}`,
      { opacity: 0, y: 10 },
      { opacity: 1, y: 0, duration: 0.6, ease: "power2.out" },
      "-=0.2"
    )
    .fromTo(
      `.${styles.progressBar}`,
      { width: '0%' },
      { 
        width: '100%', 
        duration: 2, 
        ease: "power1.inOut",
        onUpdate: function() {
          const progress = this.progress();
          const scale = 1 + Math.sin(progress * Math.PI) * 0.03;
          gsap.to(`.${styles.logo}`, { scale, duration: 0.1 });
        }
      }
    );

    mainTl.to(`.${styles.logo}`, {
      y: -15,
      scale: 1.1,
      duration: 0.5,
      ease: "back.out(1.7)"
    })
    .to(`.${styles.logo}`, {
      y: -50,
      opacity: 0,
      scale: 0.9,
      duration: 0.6,
      ease: "power2.in"
    }, "+=0.2")
    .to(`.${styles.progressBarContainer}`, {
      opacity: 0,
      y: 10,
      duration: 0.4,
      ease: "power2.in"
    }, "-=0.4");

    return () => {
      mainTl.kill();
    };
  }, []);

  if (!show) return null;

  return (
    <div ref={overlayRef} className={styles.overlay}>
      <div ref={particlesRef} className={styles.particles}></div>
      <div ref={contentRef} className={styles.content}>
        <div className={styles.logo}>
          MikuLearn
          <div className={styles.logoHighlight}></div>
        </div>
        <div className={styles.progressBarContainer}>
          <div className={styles.progressBar}></div>
        </div>
      </div>
    </div>
  );
}; 