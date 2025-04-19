import { Link } from '@argon-router/react';
import styles from './header.module.css';
import { routes } from '@shared/router';

export const Header = () => {
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
              <li>
                <Link to={routes.testGenerator}>Сгенерировать тест</Link>
              </li>
            </ul>
          </nav>
          <div className={styles.auth}>
            <Link to={routes.signIn} className={styles.loginButton}>
              Вход
            </Link>

            <Link to={routes.signUp} className={styles.registerButton}>
              Регистрация
            </Link>
          </div>
        </div>
      </header>
    </>
  );
};
