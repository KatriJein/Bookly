import styles from './page-of-book.module.scss';
// import Cover from '../../assets/images/book.png';
import Chat from './../../assets/svg/icon-chat.svg';
import List from '../../assets/svg/list-check.svg';
import { BookRating, ListPoint, Comment, ButtonAll } from '../../components';
import clsx from 'clsx';
import { Helmet } from 'react-helmet-async';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import type { Book } from '../../types';
import { getBookByIdApi } from '../../store/api/books';

export function PageOfBook() {
    const { id } = useParams<{ id: string }>();
    const [book, setBook] = useState<Book | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchBook = async () => {
            if (!id) {
                setError('ID книги не указан');
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError(null);
                const bookData = await getBookByIdApi(id);
                setBook(bookData);
            } catch (err) {
                setError(
                    err instanceof Error ? err.message : 'Произошла ошибка'
                );
            } finally {
                setLoading(false);
            }
        };

        fetchBook();
    }, [id]);

    if (loading) {
        return <div className={styles.loading}>Загрузка книги...</div>;
    }

    if (error) {
        return <div className={styles.error}>Ошибка: {error}</div>;
    }

    if (!book) {
        return <div className={styles.error}>Книга не найдена</div>;
    }

    return (
        <div className={styles.pageOfBook}>
            <Helmet>
                <title>{book.title}</title>
            </Helmet>
            <div className={styles.content}>
                <img src={book.thumbnail} alt='Cover' className={styles.cover} />
                <div className={styles.info}>
                    <div className={styles.description}>
                        <h2 className={styles.title}>{book.title}</h2>
                        <p className={styles.author}>
                            {book.authors[0].displayName}
                        </p>
                        <ListPoint
                            className={styles.list}
                            items={[
                                String(book.publishmentYear),
                                book.publisher,
                                `${book.pageCount} стр.`,
                            ]}
                        />
                        <div className={styles.tags}>
                            <span className={styles.age}>
                                {book.ageRestriction}
                            </span>
                            {book.genres.map((genre) => (
                                <span
                                    key={genre.name}
                                    className={clsx('genre')}
                                >
                                    {genre.displayName}
                                </span>
                            ))}
                            {/* <span className={clsx('genre')}>Классика</span>
                            <span className={clsx('genre')}>Фантастика</span>
                            <span className={clsx('genre')}>Научпоп</span>
                            <span className={clsx('genre')}>Драма</span> */}
                        </div>
                        <BookRating className={styles.rating} rating={book.rating} />
                        <p className={styles.descriptionText}>
                            {book.description}
                        </p>
                    </div>
                    <div className={styles.buttons}>
                        <button
                            className={clsx(
                                styles.button,
                                'button',
                                styles.pink
                            )}
                        >
                            <img
                                src={Chat}
                                alt='Chat'
                                className={styles.icon}
                            />
                            Добавить в подборку
                        </button>
                        <button
                            className={clsx(
                                styles.button,
                                'button',
                                styles.blue
                            )}
                        >
                            <img
                                src={List}
                                alt='List'
                                className={styles.icon}
                            />
                            Отметить прочитанной
                        </button>
                    </div>
                </div>
            </div>
            <div className={styles.comments}>
                <div className={styles.commentsHeader}>
                    <h3 className={styles.commentsTitle}>Отзывы</h3>
                    <ButtonAll />
                </div>

                <ul className={styles.commentsList}>
                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={4}
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />

                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'John Doe',
                        }}
                        date='15.12.2023'
                        rating={5}
                        text='Отличная книга, рекомендую всем к прочтению!'
                    />
                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={3}
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />

                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'John Doe',
                        }}
                        date='15.12.2023'
                        rating={2}
                        text='Отличная книга, рекомендую всем к прочтению!'
                    />
                </ul>
            </div>
        </div>
    );
}
