import styles from './main-banner.module.scss';
import Bookly from '../../../assets/images/big_name.png';

export function MainBanner() {
    return (
        <div className={styles.main}>
            <img className={styles.banner} src={Bookly} />
            <div className={styles.text}>
                <h1 className={styles.title}>Букли</h1>
                <h2 className={styles.subtitle}>
                    Сервис для подбора книг и курсов по вашим предпочтениям
                </h2>
                <button className={`button ${styles.button}`}>
                    Настроить рекомендации
                </button>
            </div>
        </div>
    );
}
