import styles from './comment.module.scss';
import Person from '../../../assets/images/person.jpg';
import Star from '../../../assets/svg/star.svg';
import StarGrey from '../../../assets/svg/star_grey.svg';
import { EditButton } from '../../uikit';

interface CommentProps {
    user: {
        avatar: string;
        name: string;
    };
    date: string;
    rating: number;
    text: string;
    editable?: boolean;
    onEdit?: () => void;
}

export function Comment(props: CommentProps) {
    const { user, date, rating, text, editable = false, onEdit } = props;
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
                <div className={styles.rate}>
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
                    {editable && <EditButton onClick={onEdit} />}
                </div>
            </div>
            <p className={styles.text}>{text}</p>
        </div>
    );
}
