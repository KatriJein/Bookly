import { Link, NavLink } from 'react-router-dom';
import styles from './footer.module.scss';
import Logo from '../../assets/svg/logo_dark.svg';

export function Footer() {
    return (
        <div className={styles.wrapper}>
            <div className={styles.footer}>
                <Link to="/" className={styles.logo}>
                    <img src={Logo} alt='Logo' className={styles.image} />
                </Link>

                <nav className={styles.nav}>
                    <NavLink 
                        to="/books"
                        className={({ isActive }) => 
                            isActive ? `${styles.link} ${styles.active}` : styles.link
                        }
                    >
                        Книги
                    </NavLink>
                    <NavLink 
                        to="/favorites"
                        className={({ isActive }) => 
                            isActive ? `${styles.link} ${styles.active}` : styles.link
                        }
                    >
                        Избранное
                    </NavLink>
                    <NavLink 
                        to="/profile"
                        className={({ isActive }) => 
                            isActive ? `${styles.link} ${styles.active}` : styles.link
                        }
                    >
                        Профиль
                    </NavLink>
                </nav>
            </div>
        </div>
    );
}