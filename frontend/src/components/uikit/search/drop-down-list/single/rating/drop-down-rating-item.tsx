import styles from './drop-down-rating-item.module.scss';
import Star from '../../../../../../assets/svg/star.svg';
import StarGrey from '../../../../../../assets/svg/star_grey.svg';
import clsx from 'clsx';

interface DropDownRatingItemProps {
    text: string;
    rating: number;
    onClick?: () => void;
    className?: string;
}

export function DropDownRatingItem({
    text,
    rating,
    onClick,
    className = '',
}: DropDownRatingItemProps) {
    const stars = Array.from({ length: 5 }, (_, index) => {
        const isActive = index < rating;
        return (
            <img
                key={index}
                src={isActive ? Star : StarGrey}
                alt={isActive ? 'Активная звезда' : 'Неактивная звезда'}
                className={styles.star}
            />
        );
    });

    return (
        <div
            className={clsx(styles.dropDownRatingItem, className)}
            onClick={onClick}
        >
            <span className={styles.text}>{text}</span>
            <div className={styles.starsContainer}>{stars}</div>
        </div>
    );
}
