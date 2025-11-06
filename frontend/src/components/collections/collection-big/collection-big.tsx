import styles from './collection-big.module.scss';
import Cover from '../../../assets/images/collection-big.png';
import MousePointer from '../../../assets/svg/mouse-pointer.svg';
import Person from '../../../assets/images/person.jpg';
import Star from '../../../assets/svg/star.svg';

export function CollectionBig() {
    return (
        <div className={styles.collection}>
            <a href='#' className={styles.link}>
                <div className={styles.cover}>
                    <img src={Cover} alt='Cover' className={styles.image} />
                    <div className={styles.point}>
                        <img
                            src={MousePointer}
                            alt='Mouse pointer'
                            className={styles.icon}
                        />
                    </div>
                </div>
                <div className={styles.title}>
                    <p className={styles.text}>Саморазвитие</p>
                    <div className={styles.rating}>
                        <img src={Star} alt='Star' className={styles.star} />
                        <p className={styles.value}>5,0</p>
                    </div>
                </div>
            </a>

            <a href='#' className={styles.author}>
                <img src={Person} alt='Person' className={styles.avatar} />
                <p className={styles.name}>Max Verstappenov</p>
            </a>
        </div>
    );
}
