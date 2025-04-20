import { Link } from 'react-router-dom';
import styles from './header.module.css';
import { store } from '@shared/store/store';
import { observer } from 'mobx-react-lite';

export const Header = observer(() => {
  const isAuthenticated = store.isAuth;

  return (
    <>
      <header className={styles.Header}>
        <div className={styles.container}>
          <div className={styles.logo}>
            <Link to="/" className={styles.logoLink}>
              MikuLearn
            </Link>
          </div>

          {/* это я буду делать только за деньги */}
          {/* <nav className={styles.navigation}>
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
          </nav> */}
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
});
