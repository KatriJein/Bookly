import styles from './collection-small.module.scss';
import Cover from '../../../assets/images/collection-big.png';
import MousePointer from '../../../assets/svg/mouse-pointer.svg';
import Star from '../../../assets/svg/star.svg';
import { useNavigate } from 'react-router-dom';
import type { BookCollection } from '../../../types';

interface CollectionSmallProps {
    collection: BookCollection;
}

export function CollectionSmall({ collection }: CollectionSmallProps) {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/collection-page/${collection.id}`, { state: { collection } });
    };

    const coverUrl = collection.coverUrl || Cover; // ← заглушка
    const isPublic = collection.isPublic;
    const rating = collection.rating > 0 ? collection.rating.toFixed(1) : '–';

    return (
        <div className={styles.collection}>
            <a onClick={handleClick} className={styles.link}>
                <div className={styles.cover}>
                    <img src={coverUrl} alt='Cover' className={styles.image} />
                    {isPublic && (
                        <span className={styles.status}>Публичная</span>
                    )}
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
                        <p className={styles.text}>{collection.title}</p>
                        <div className={styles.rating}>
                            <img
                                src={Star}
                                alt='Star'
                                className={styles.star}
                            />
                            <p className={styles.value}>{rating}</p>
                        </div>
                    </div>
                    <p className={styles.count}>{collection.booksCount} книг</p>
                </div>
            </a>
        </div>
    );
}
