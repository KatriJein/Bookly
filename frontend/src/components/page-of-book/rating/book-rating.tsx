import styles from './book-rating.module.scss';
import Star from '../../../assets/svg/star.svg';
import StarGrey from '../../../assets/svg/star_grey.svg';
import clsx from 'clsx';

interface BookRatingProps {
    rating: number;
    className?: string;
}

export function BookRating(props: BookRatingProps) {
    const { rating, className } = props;
    const roundedRating = Math.round(rating * 2) / 2;
    const filledStars = Math.ceil(roundedRating);
    const greyStars = 5 - filledStars;

    return (
        <div className={clsx(styles.rating, className)}>
            <p>{rating.toFixed(1).replace('.', ',')}</p>
            <div className={styles.stars}>
                {Array.from({ length: filledStars }, (_, index) => (
                    <img
                        key={`star-${index}`}
                        src={Star}
                        alt='Star'
                        className={styles.star}
                    />
                ))}

                {Array.from({ length: greyStars }, (_, index) => (
                    <img
                        key={`grey-star-${index}`}
                        src={StarGrey}
                        alt='Grey star'
                        className={styles.star}
                    />
                ))}
            </div>
        </div>
    );
}
