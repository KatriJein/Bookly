import { useNavigate } from 'react-router-dom';
import styles from './collection-small.module.scss';

interface CollectionSmallMoreProps {
    count: number;
}

export function CollectionSmallMore({ count }: CollectionSmallMoreProps) {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate('/my-collections');
    };

    return (
        <div className={styles.collection}>
            <a onClick={handleClick} className={styles.link}>
                <div className={styles.coverMore}>
                    <div className={styles.blueBackground}></div>
                    <div className={styles.point}>
                        <p className={styles.number}>+{count}</p>
                    </div>
                </div>
            </a>
        </div>
    );
}
