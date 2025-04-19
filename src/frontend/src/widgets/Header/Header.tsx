import styles from './Header.module.css'
import { Link } from 'react-router-dom'

export const Header = () => {
    return (
        <>
            <header className={styles.Header}>  
                <div className={styles.container}>
                    <div className={styles.logo}>
                        <Link to="/" className={styles.logoLink}>MikuLearn</Link>
                    </div>
                    <nav className={styles.navigation}>
                        <ul>
                            <li><Link to="/">Главная</Link></li>
                            <li><Link to="/dashboard">Мои курсы</Link></li>
                            <li><Link to="/about">Топ учеников</Link></li>
                        </ul>
                    </nav>
                    <div className={styles.auth}>
                        <Link to="/login" className={styles.loginButton}>Вход</Link>
                        <Link to="/register" className={styles.registerButton}>Регистрация</Link>
                    </div>
                </div>
            </header>
        </>
    );
};