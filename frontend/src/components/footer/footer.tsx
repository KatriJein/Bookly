import styles from './footer.module.scss';
import Logo from '../../assets/svg/logo_dark.svg';

export function Footer() {
    return (
        <div className={styles.wrapper}>
            <div className={styles.footer}>
                <a href='#' className={styles.logo}>
                    <img src={Logo} alt='Logo' className={styles.image} />
                </a>

                <nav className={styles.nav}>
                    <a href='#'>Книги</a>
                    <a href='#'>Избранное</a>
                    <a href='#'>Профиль</a>
                </nav>
            </div>
        </div>
    );
}
