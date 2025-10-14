import Logo from '../assets/svg/logo_dark.svg';
import styles from './header.module.scss';

export function Header() {
    return (
        <header className={styles.header}>
            <a href='#'>
                <img className={styles.logo} src={Logo} alt='Logo' />
            </a>
            <nav className={styles.nav}>
                <a href='#'>Книги</a>
                <a href='#'>Избранное</a>
                <a href='#'>Профиль</a>
            </nav>
        </header>
    );
}

