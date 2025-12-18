// CreateReview.tsx

import { useState } from 'react';
import styles from './create-review.module.scss';
import StarFull from '../../assets/svg/star.svg';
import StarEmpty from '../../assets/svg/star_line.svg';
// import clsx from 'clsx';
import { useSelector } from 'react-redux';
import { selectUser } from '../../store'; // Убедись, что путь правильный
import clsx from 'clsx';

interface CreateReviewProps {
    onSubmit: (review: string, rating: number) => void;
    placeholder?: string;
    submitButtonText?: string;
}

export const CreateReview = ({
    onSubmit,
    placeholder = 'Что вы думаете о книге?',
    submitButtonText = 'Отправить',
}: CreateReviewProps) => {
    const currentUser = useSelector(selectUser);
    const [reviewText, setReviewText] = useState('');
    const [rating, setRating] = useState(0); // Выбранная оценка
    const [hoverRating, setHoverRating] = useState(0); // Оценка при наведении

    if (!currentUser) {
        return null;
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (reviewText.trim() && rating > 0) {
            onSubmit(reviewText, rating);
            setReviewText('');
            setRating(0);
        }
    };

    const handleStarClick = (starValue: number) => {
        setRating(starValue);
    };

    const displayRating = hoverRating || rating; // При наведении — hover, иначе — выбранная

    return (
        <div className={styles.reviewContainer}>
            <div className={styles.header}>
                <div className={styles.userInfo}>
                    <img
                        src={currentUser.avatarUrl || 'https://via.placeholder.com/40'}
                        alt="Аватар"
                        className={styles.avatar}
                    />
                    <span className={styles.userName}>{currentUser.login}</span>
                </div>
                <div className={styles.ratingStars}>
                    {[1, 2, 3, 4, 5].map((star) => (
                        <button
                            key={star}
                            type="button"
                            className={styles.starButton}
                            onClick={() => handleStarClick(star)}
                            onMouseEnter={() => setHoverRating(star)}
                            onMouseLeave={() => setHoverRating(0)}
                            aria-label={`Оценить на ${star} звёзд`}
                        >
                            {/* {star <= displayRating ? (
                                <StarFull />
                            ) : (
                                <StarEmpty />
                            )} */}
                            <img className={styles.star} src={star <= displayRating ? StarFull : StarEmpty} alt="Star" />
                        </button>
                    ))}
                </div>
            </div>

            <form onSubmit={handleSubmit} className={styles.form}>
                <textarea
                    value={reviewText}
                    onChange={(e) => setReviewText(e.target.value)}
                    placeholder={placeholder}
                    className={styles.textarea}
                    rows={4}
                />
                <button
                    type="submit"
                    className={clsx('pink', 'button', styles.submitButton)}
                    disabled={!reviewText.trim() || rating === 0}
                >
                    {submitButtonText}
                </button>
            </form>
        </div>
    );
};