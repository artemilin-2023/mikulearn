import { Link } from 'react-router-dom';
import { useEffect, useState } from 'react';
import styles from './NotFoundPage.module.css';

export const NotFoundPage = () => {
    const [position, setPosition] = useState({ x: 0, y: 0 });
    
    // Эффект для отслеживания движения мыши
    useEffect(() => {
        const handleMouseMove = (e: MouseEvent) => {
            setPosition({
                x: (e.clientX / window.innerWidth - 0.5) * 20,
                y: (e.clientY / window.innerHeight - 0.5) * 20
            });
        };
        
        window.addEventListener('mousemove', handleMouseMove);
        
        return () => {
            window.removeEventListener('mousemove', handleMouseMove);
        };
    }, []);

    return (
        <div className={styles.notFoundContainer}>
            <div 
                className={styles.content}
                style={{ 
                    transform: `translate(${position.x * 0.5}px, ${position.y * 0.5}px)` 
                }}
            >
                <h1 className={styles.title}>404</h1>
                <div className={styles.message}>
                    <h2>Страница не найдена</h2>
                    <p>Упс! Кажется, вы заблудились в цифровом пространстве.</p>
                </div>
                <Link to="/" className={styles.homeButton}>
                    Вернуться на главную
                </Link>
                
                <div className={styles.particles}>
                    {[...Array(6)].map((_, i) => (
                        <div 
                            key={i} 
                            className={styles.particle}
                            style={{ 
                                animationDelay: `${i * 0.8}s`,
                                transform: `translate(${position.x * (i % 3 + 1)}px, ${position.y * (i % 3 + 1)}px)` 
                            }}
                        />
                    ))}
                </div>
            </div>
        </div>
    );
};   