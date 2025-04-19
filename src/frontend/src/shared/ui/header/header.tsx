import { Link } from '@argon-router/react';
import styles from './header.module.css';
import { routes } from '@shared/router';
import { useUnit } from 'effector-react';
import { $isAuth } from '@shared/user/user';

export const Header = () => {
  const isAuthenticated = useUnit($isAuth);

  return (
    <>
      <header className={styles.Header}>
        <div className={styles.container}>
          <div className={styles.logo}>
            <Link to={routes.home} className={styles.logoLink}>
              MikuLearn
            </Link>
          </div>
          <nav className={styles.navigation}>
            <ul>
              <li>
                <Link to={routes.home}>Главная</Link>
              </li>
              <li>
                <Link to={routes.dashboard}>Мои курсы</Link>
              </li>
              <li>
                <Link to={routes.about}>Топ учеников</Link>
              </li>
            </ul>
          </nav>
          <div className={styles.auth}>
            {
              isAuthenticated ? (
                <Link to={routes.personalCabinet} className={styles.personalCabinetButton}>
                  Личный кабинет
                </Link>
              ) : (
                <>
                  <Link to={routes.signIn} className={styles.loginButton}>
                    Вход
                  </Link>

                  <Link to={routes.signUp} className={styles.registerButton}>
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
