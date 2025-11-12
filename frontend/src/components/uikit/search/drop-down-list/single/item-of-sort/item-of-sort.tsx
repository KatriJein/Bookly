import styles from './item-of-sort.module.scss';
import ArrowDown from '../../../../../../assets/svg/arrow_down.svg';
import ArrowUp from '../../../../../../assets/svg/arrow_up.svg';
import { clsx } from 'clsx';

interface ItemOfSortProps {
    title: string;
    sortOrder?: 'asc' | 'desc';
    onClick?: () => void;
    className?: string;
}

export function ItemOfSort({
    title,
    sortOrder = 'asc',
    onClick,
    className = '',
}: ItemOfSortProps) {
    const arrowIcon = sortOrder === 'asc' ? ArrowUp : ArrowDown;
    const arrowAlt =
        sortOrder === 'asc'
            ? 'Сортировка по возрастанию'
            : 'Сортировка по убыванию';

    return (
        <div className={clsx(styles.itemOfSort, className)} onClick={onClick}>
            <span className={styles.title}>{title}</span>
            <img src={arrowIcon} alt={arrowAlt} className={styles.arrow} />
        </div>
    );
}
