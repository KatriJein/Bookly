import styles from './button-all.module.scss';
import Arrow from '../../../assets/svg/arrow-right.svg';

export function ButtonAll() {
    return (
        <button className={styles.button}>
            <span>Смотреть все</span>
            <img src={Arrow} alt='Arrow' />
        </button>
    );
}