import styles from './comment.module.scss';
import Person from '../../../assets/images/person.jpg';
import Star from '../../../assets/svg/star.svg';
import StarGrey from '../../../assets/svg/star_grey.svg';

interface CommentProps {
    user: {
        avatar: string;
        name: string;
    };
    date: string;
    rating: number;
    text: string;
}

export function Comment({ user, date, rating, text }: CommentProps) {
    const stars = Array.from({ length: 5 }, (_, index) => {
        return index < rating ? Star : StarGrey;
    });

    return (
        <div className={styles.comment}>
            <div className={styles.header}>
                <a href='#' className={styles.user}>
                    <img
                        src={Person}
                        alt='Фотография пользователя'
                        className={styles.avatar}
                    />
                    <div className={styles.info}>
                        <p className={styles.name}>{user.name}</p>
                        <p className={styles.date}>{date}</p>
                    </div>
                </a>
                <div className={styles.rating}>
                    {stars.map((star, index) => (
                        <img
                            key={index}
                            src={star}
                            alt='Звезда'
                            className={styles.star}
                        />
                    ))}
                </div>
            </div>
            <p className={styles.text}>{text}</p>
        </div>
    );
}
