import styles from './book.module.scss';
import Cover from '../../assets/images/book.png';
import Like from '../../assets/svg/heart-outline.svg';
import Star from '../../assets/svg/star.svg';
import Point from '../../assets/svg/point.svg';
import Menu from '../../assets/svg/menu.svg';

export function Book() {
    return (
        <div className={styles.book}>
            <img src={Cover} alt='Cover' className={styles.cover} />

            <button className={styles.like}>
                <img src={Like} alt='Like' className={styles.icon} />
            </button>

            <div className={styles.info}>
                <div className={styles.description}>
                    <p className={styles.name}>451 градус по Фаренгейту</p>
                    <p className={styles.author}>Джон Голсоурси</p>
                    <div className={styles.year}>
                        <span>2025</span>
                        <img src={Point} alt='Point' />
                        <div className={styles.rating}>
                            <img
                                src={Star}
                                alt='Star'
                                className={styles.star}
                            />
                            <span className={styles.value}>5,0</span>
                        </div>
                    </div>
                    <ul className={styles.tags}>
                        <li className={styles.tag}>Драма</li>
                        <li className={styles.tag}>Фантастика</li>
                    </ul>
                </div>
                <button className={styles.menu}>
                    <img src={Menu} alt='Menu' className={styles.icon} />
                </button>
            </div>

            <button className={`button ${styles.button}`}>Подробнее</button>
        </div>
    );
}
