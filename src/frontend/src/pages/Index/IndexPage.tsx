import { Link } from 'react-router-dom';
import { FaLaptopCode, FaArrowRight, FaUsers, FaBook, FaLightbulb } from 'react-icons/fa';
import { useEffect, useState } from 'react';
import styles from './IndexPage.module.css';

export const IndexPage = () => {
    const [scrolled, setScrolled] = useState(false);

    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    return (
        <div className={styles.indexPage}>
            <section className={`${styles.hero} ${scrolled ? styles.scrolled : ''}`}>
                <div className={styles.heroContent}>
                    <div className={styles.titleWrapper}>
                        <h1 className={styles.title}>
                            Учитесь <span className={styles.highlight}>эффективно</span>
                        </h1>
                        <h1 className={styles.title}>
                            Развивайтесь <span className={styles.highlight}>быстро</span>
                        </h1>
                    </div>
                    <p className={styles.subtitle}>
                        Современная платформа для онлайн-обучения с персонализированным подходом
                    </p>
                    <div className={styles.cta}>
                        <Link to="/register" className={styles.primaryButton}>
                            Начать обучение <FaArrowRight className={styles.buttonIcon} />
                        </Link>
                    </div>
                </div>
                <div className={styles.heroVisual}>
                    <div className={styles.gridContainer}>
                        <div className={`${styles.gridItem} ${styles.gridItem3}`}>
                            <div className={styles.iconWrapper}>
                                <FaLaptopCode />
                            </div>
                            <h3>Интерактивное обучение</h3>
                        </div>
                        <div className={`${styles.gridItem} ${styles.gridItem4}`}>
                            <div className={styles.iconWrapper}>
                                <FaUsers />
                            </div>
                            <h3>Сообщество</h3>
                        </div>
                        <div className={`${styles.gridItem} ${styles.gridItem5}`}>
                            <div className={styles.iconWrapper}>
                                <FaBook />
                            </div>
                            <h3>Отслеживание прогресса</h3>
                        </div>
                        <div className={`${styles.gridItem} ${styles.gridItem6}`}>
                            <div className={styles.iconWrapper}>
                                <FaLightbulb />
                            </div>
                            <h3>Инновационные методики</h3>
                        </div>
                    </div>
                </div>
            </section>

            {/* я хз нужно ли */}
            {/* <section className={`${styles.stats} animate-on-scroll`}>
                <div className={styles.statItem}>
                    <span className={styles.statNumber}>100+</span>
                    <span className={styles.statLabel}>Курсов</span>
                </div>
                <div className={styles.statItem}>
                    <span className={styles.statNumber}>50K+</span>
                    <span className={styles.statLabel}>Студентов</span>
                </div>
                <div className={styles.statItem}>
                    <span className={styles.statNumber}>95%</span>
                    <span className={styles.statLabel}>Успешных выпускников</span>
                </div>
            </section> */}

            <section className={`${styles.featuredCourses} animate-on-scroll`}>
                <h2 className={styles.sectionTitle}>Популярные курсы</h2>
                <div className={styles.courseGrid}>
                    <div className={`${styles.courseCard} animate-on-scroll`}>
                        <div className={`${styles.courseImage} ${styles.courseImage1}`}></div>
                        <div className={styles.courseContent}>
                            <span className={styles.courseTag}>Программирование</span>
                            <h3>Основы Python</h3>
                            <p>Изучите основы программирования на Python с нуля</p>
                            <Link to="/courses/python" className={styles.courseLink}>
                                Подробнее <FaArrowRight />
                            </Link>
                        </div>
                    </div>
                    <div className={`${styles.courseCard} animate-on-scroll`}>
                        <div className={`${styles.courseImage} ${styles.courseImage2}`}></div>
                        <div className={styles.courseContent}>
                            <span className={styles.courseTag}>Дизайн</span>
                            <h3>UI/UX Дизайн</h3>
                            <p>Создавайте привлекательные и удобные интерфейсы</p>
                            <Link to="/courses/uiux" className={styles.courseLink}>
                                Подробнее <FaArrowRight />
                            </Link>
                        </div>
                    </div>
                    <div className={`${styles.courseCard} animate-on-scroll`}>
                        <div className={`${styles.courseImage} ${styles.courseImage3}`}></div>
                        <div className={styles.courseContent}>
                            <span className={styles.courseTag}>Аналитика</span>
                            <h3>Анализ данных</h3>
                            <p>Научитесь анализировать данные и принимать решения</p>
                            <Link to="/courses/data" className={styles.courseLink}>
                                Подробнее <FaArrowRight />
                            </Link>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    );
};   