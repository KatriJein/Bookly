import { Link, NavLink } from 'react-router-dom';
import Logo from '../../assets/svg/logo_dark.svg';
import styles from './header.module.scss';

export function Header() {
    return (
        <header className={styles.header}>
            <Link to='/'>
                <img className={styles.logo} src={Logo} alt='Logo' />
            </Link>
            <nav className={styles.nav}>
                <NavLink
                    to='/books'
                    className={({ isActive }) =>
                        isActive
                            ? `${styles.link} ${styles.active}`
                            : styles.link
                    }
                >
                    Книги
                </NavLink>
                <NavLink
                    to='/favorites'
                    className={({ isActive }) =>
                        isActive
                            ? `${styles.link} ${styles.active}`
                            : styles.link
                    }
                >
                    Избранное
                </NavLink>
                <NavLink
                    to='/profile'
                    className={({ isActive }) =>
                        isActive
                            ? `${styles.link} ${styles.active}`
                            : styles.link
                    }
                >
                    Профиль
                </NavLink>
            </nav>
        </header>
    );
}
