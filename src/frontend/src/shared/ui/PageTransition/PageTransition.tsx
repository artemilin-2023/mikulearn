import { useRef, useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import gsap from 'gsap';
import styles from './PageTransition.module.css';

interface PageTransitionProps {
  children: React.ReactNode;
}

export const PageTransition = ({ children }: PageTransitionProps) => {
  const location = useLocation();
  const circleRef = useRef<HTMLDivElement>(null);
  const contentRef = useRef<HTMLDivElement>(null);
  const oldContentRef = useRef<HTMLDivElement>(null);
  const [displayChildren, setDisplayChildren] = useState(children);
  const [transitioning, setTransitioning] = useState(false);
  
  // Функция для расчета размера круга, чтобы он покрыл весь экран
  const calculateCircleSize = () => {
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    return Math.sqrt(Math.pow(viewportWidth, 2) + Math.pow(viewportHeight, 2)) * 1.5;
  };
  
  useEffect(() => {
    // Если это первый рендер или нет изменения маршрута, не делаем анимацию
    if (!transitioning && location.pathname === oldContentRef.current?.dataset.path) {
      return;
    }
    
    setTransitioning(true);
    
    // Создаем таймлайн анимации
    const timeline = gsap.timeline({
      onComplete: () => {
        setTransitioning(false);
        // После завершения анимации обновляем отображаемый контент
        setDisplayChildren(children);
      }
    });
    
    // Сохраняем старый контент
    if (!oldContentRef.current?.dataset.path) {
      oldContentRef.current?.setAttribute('data-path', location.pathname);
    }
    
    // Устанавливаем начальное состояние
    timeline.set(circleRef.current, {
      scale: 0,
      opacity: 1
    });
    
    timeline.set(contentRef.current, {
      opacity: 0
    });
    
    // Анимируем расширение круга
    timeline.to(circleRef.current, {
      scale: 1,
      duration: 1.2,
      ease: "power3.out"
    });
    
    // Показываем новый контент внутри круга
    timeline.to(contentRef.current, {
      opacity: 1,
      duration: 0.5,
      ease: "power2.inOut"
    }, "-=0.8");
    
    // Обновляем отображаемый контент после начала анимации
    if (children !== displayChildren) {
      setDisplayChildren(children);
    }
    
    return () => {
      timeline.kill();
    };
  }, [children, location.pathname]);
  
  return (
    <div className={styles.pageTransitionWrapper}>
      {/* Старый контент (фон) */}
      <div 
        ref={oldContentRef} 
        className={styles.oldContent}
        data-path={location.pathname}
      >
        {transitioning ? displayChildren : null}
      </div>
      
      {/* Круг с новым контентом */}
      <div 
        ref={circleRef} 
        className={styles.transitionCircle}
        style={{ width: `${calculateCircleSize()}px`, height: `${calculateCircleSize()}px` }}
      >
        <div 
          ref={contentRef} 
          className={styles.newContent}
        >
          {children}
        </div>
      </div>
    </div>
  );
}; 