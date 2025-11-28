import styles from './collection-small.module.scss';
import Cover from '../../../assets/images/collection-big.png';
import MousePointer from '../../../assets/svg/mouse-pointer.svg';
import Star from '../../../assets/svg/star.svg';
import { useNavigate } from 'react-router-dom';

export function CollectionSmall() {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/collection-page`);
    };

    return (
        <div className={styles.collection}>
            <a onClick={handleClick} className={styles.link}>
                <div className={styles.cover}>
                    <img src={Cover} alt='Cover' className={styles.image} />
                    <span className={styles.status}>Публичная</span>
                    <div className={styles.point}>
                        <img
                            src={MousePointer}
                            alt='Mouse pointer'
                            className={styles.icon}
                        />
                    </div>
                </div>
                <div className={styles.info}> 
                    <div className={styles.title}>
                        <p className={styles.text}>Саморазвитие</p>
                        <div className={styles.rating}>
                            <img
                                src={Star}
                                alt='Star'
                                className={styles.star}
                            />
                            <p className={styles.value}>5,0</p>
                        </div>
                    </div>
                    <p className={styles.count}>20 книг</p>
                </div>
            </a>
        </div>
    );
}
