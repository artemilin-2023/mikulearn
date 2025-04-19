import { Link } from 'react-router-dom';
import styles from './header.module.css';
import { useUnit } from 'effector-react';
import { $isAuth } from '@shared/user/user';

export const Header = () => {
  const isAuthenticated = useUnit($isAuth);

  return (
    <>
      <header className={styles.Header}>
        <div className={styles.container}>
          <div className={styles.logo}>
            <Link to="/" className={styles.logoLink}>
              MikuLearn
            </Link>
          </div>
          <nav className={styles.navigation}>
            <ul>
              <li>
                <Link to="/">Главная</Link>
              </li>
              <li>
                <Link to="/dashboard">Мои курсы</Link>
              </li>
              <li>
                <Link to="/404">Топ учеников</Link>
              </li>
            </ul>
          </nav>
          <div className={styles.auth}>
            {
              isAuthenticated ? (
                <Link to="/personal-cabinet" className={styles.personalCabinetButton}>
                  Личный кабинет
                </Link>
              ) : (
                <>
                  <Link to="/sign-in" className={styles.loginButton}>
                    Вход
                  </Link>

                  <Link to="/sign-up" className={styles.registerButton}>
                    Регистрация
                  </Link>
                </>
              )
            }

          </div>
        </div>
      </header>
    </>
  );
};
