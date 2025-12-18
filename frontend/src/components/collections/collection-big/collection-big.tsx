import styles from './collection-big.module.scss';
import Cover from '../../../assets/images/collection-big.png';
import MousePointer from '../../../assets/svg/mouse-pointer.svg';
import Person from '../../../assets/images/person.jpg';
import Star from '../../../assets/svg/star.svg';
import { useNavigate } from 'react-router-dom';
import type { BookCollection } from '../../../types';
import DefaultUser from '../../../assets/images/default-user.png';

interface CollectionBigProps {
    collection?: BookCollection;
}

export function CollectionBig({ collection }: CollectionBigProps) {
    const navigate = useNavigate();

    const handleClick = () => {
        if (collection) {
            navigate(`/collection-page/${collection.id}`, {
                state: { collection },
            });
        }
    };

    if (!collection) {
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
                            <img
                                src={Star}
                                alt='Star'
                                className={styles.star}
                            />
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

    // Динамические данные
    const coverUrl = collection.coverUrl || Cover;
    const avatarUrl = collection.userInfo.avatarUrl
        ? collection.userInfo.avatarUrl.startsWith('person')
            ? `https://bookly-files-bucket.s3.yandexcloud.net/${collection.userInfo.avatarUrl}`
            : collection.userInfo.avatarUrl
        : DefaultUser;
    const rating = collection.rating > 0 ? collection.rating.toFixed(1) : '–';
    const isPublic = collection.isPublic;

    return (
        <div className={styles.collection}>
            <a onClick={handleClick} className={styles.link}>
                <div className={styles.cover}>
                    <img src={coverUrl} alt='Cover' className={styles.image} />
                    {isPublic && (
                        <div className={styles.point}>
                            <img
                                src={MousePointer}
                                alt='Mouse pointer'
                                className={styles.icon}
                            />
                        </div>
                    )}
                </div>
                <div className={styles.title}>
                    <p className={styles.text}>{collection.title}</p>
                    <div className={styles.rating}>
                        <img src={Star} alt='Star' className={styles.star} />
                        <p className={styles.value}>{rating}</p>
                    </div>
                </div>
            </a>

            <a href={`/profile/${collection.userId}`} className={styles.author}>
                <img
                    src={avatarUrl}
                    alt={collection.userInfo.login}
                    className={styles.avatar}
                />
                <p className={styles.name}>{collection.userInfo.login}</p>
            </a>
        </div>
    );
}
