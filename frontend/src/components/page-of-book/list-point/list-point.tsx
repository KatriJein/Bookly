import styles from './list-point.module.scss';
import Point from '../../../assets/svg/point.svg';
import clsx from 'clsx';

interface ListPointProps {
    items: string[];
    className?: string;
}

export function ListPoint(props: ListPointProps) {
    const { items, className } = props;
    return (
        <div className={clsx(styles.list, className)}>
            {items.map((item, index) => (
                <>
                    <p key={index} className={styles.item}>
                        {item}
                    </p>
                    {index < items.length - 1 && (
                        <img src={Point} alt='Point' className={styles.point} />
                    )}
                </>
            ))}
        </div>
    );
}
