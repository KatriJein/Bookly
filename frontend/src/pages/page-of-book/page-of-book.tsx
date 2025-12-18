import styles from './page-of-book.module.scss';
// import Cover from '../../assets/images/book.png';
import Chat from './../../assets/svg/icon-chat.svg';
import List from '../../assets/svg/list-check.svg';
import {
    BookRating,
    ListPoint,
    Comment,
    ButtonAll,
    CreateReview,
    Book,
    CollectionMenu,
} from '../../components';
import clsx from 'clsx';
import { Helmet } from 'react-helmet-async';
import { useParams } from 'react-router-dom';
import { useEffect, useRef, useState } from 'react';
import type {
    BookCollection,
    Book as BookType,
    Review,
    ShortBook,
} from '../../types';
import {
    createReviewApi,
    getBookByIdApi,
    getBookReviewsApi,
    getSimilarBooksApi,
} from '../../store/api/books';
import {
    addBookToDynamicCollection,
    addBookToStaticCollection,
    fetchBooksCollections,
    selectBooksCollections,
    selectUser,
    useDispatch,
    useSelector,
} from '../../store';
import { toast } from 'react-toastify';

export function PageOfBook() {
    const collectionButtonRef = useRef<HTMLButtonElement>(null);
    const [isCollectionMenuOpen, setIsCollectionMenuOpen] = useState(false);
    const dispatch = useDispatch();
    const collections = useSelector(selectBooksCollections);

    const { id } = useParams<{ id: string }>();
    const [book, setBook] = useState<BookType | null>(null);
    const [reviews, setReviews] = useState<Review[]>([]);
    const [similarBooks, setSimilarBooks] = useState<ShortBook[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const currentUser = useSelector(selectUser);

    useEffect(() => {
        if (currentUser && collections.length === 0) {
            dispatch(fetchBooksCollections({ userId: currentUser.id }));
        }
    }, [currentUser, collections.length, dispatch]);

    const handleMarkAsRead = () => {
        if (!book) return;
        dispatch(
            addBookToStaticCollection({
                collectionName: 'Прочитано',
                bookId: book.id,
            })
        );
        toast.success('Книга добавлена в "Прочитано"');
    };

    const handleOpenCollectionMenu = () => {
        setIsCollectionMenuOpen(true);
    };

    // Обработчик закрытия меню
    // const handleCloseCollectionMenu = () => {
    //     setIsCollectionMenuOpen(false);
    // };

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (
                collectionButtonRef.current &&
                !collectionButtonRef.current.contains(event.target as Node)
            ) {
                setIsCollectionMenuOpen(false);
            }
        }

        if (isCollectionMenuOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isCollectionMenuOpen]);

    const handleSelectCollection = (collection: BookCollection) => {
        if (!book) return;

        if (collection.id) {
            dispatch(
                addBookToDynamicCollection({
                    collectionId: collection.id,
                    bookId: book.id,
                })
            );
        } else {
            dispatch(
                addBookToStaticCollection({
                    collectionName: collection.title,
                    bookId: book.id,
                })
            );
        }

        toast.success(`Книга добавлена в "${collection.title}"`);
        setIsCollectionMenuOpen(false);
    };

    useEffect(() => {
        const fetchBookData = async () => {
            if (!id) {
                setError('ID книги не указан');
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError(null);

                // Загружаем книгу
                const bookData = await getBookByIdApi(id);
                setBook(bookData);

                // Загружаем отзывы
                const reviewsData = await getBookReviewsApi(id);
                setReviews(reviewsData);

                // Загружаем похожие книги
                const similarBooksData = await getSimilarBooksApi(id);
                setSimilarBooks(similarBooksData);
            } catch (err) {
                setError(
                    err instanceof Error ? err.message : 'Произошла ошибка'
                );
            } finally {
                setLoading(false);
            }
        };

        fetchBookData();
    }, [id]);

    const handleReviewSubmit = async (text: string, rating: number) => {
        if (!id || !currentUser) {
            alert(
                'Невозможно отправить отзыв: нет данных о книге или пользователе'
            );
            return;
        }

        try {
            // 1. Отправляем отзыв на сервер
            await createReviewApi({
                text,
                rating,
                bookId: id,
                // userId не нужен в теле — сервер берёт его из токена
            });

            // 2. Обновляем список отзывов (или просто перезагружаем их)
            const updatedReviews = await getBookReviewsApi(id);
            setReviews(updatedReviews);

            // 3. Уведомление
            // Используй toast вместо alert, если у тебя настроен react-toastify
            toast.success('Отзыв отправлен!');
        } catch (err) {
            const message =
                err instanceof Error
                    ? err.message
                    : 'Не удалось отправить отзыв';
            toast.error(message);
        }
    };

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
                <img
                    src={book.thumbnail}
                    alt='Cover'
                    className={styles.cover}
                />
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

                        <BookRating
                            className={styles.rating}
                            rating={book.rating}
                        />
                        <p className={styles.descriptionText}>
                            {book.description}
                        </p>
                    </div>
                    <div className={styles.buttons}>
                        <div className={styles.buttonWithMenu}>
                            <button
                                ref={collectionButtonRef} 
                                className={clsx(
                                    styles.button,
                                    'button',
                                    styles.pink
                                )}
                                onClick={handleOpenCollectionMenu}
                            >
                                <img
                                    src={Chat}
                                    alt='Chat'
                                    className={styles.icon}
                                />
                                Добавить в подборку
                            </button>
                            {isCollectionMenuOpen && (
                                <CollectionMenu
                                    className={styles.popup} // 👈 Используем тот же класс `.popup`
                                    onClose={() =>
                                        setIsCollectionMenuOpen(false)
                                    }
                                    collections={collections}
                                    onSelect={handleSelectCollection}
                                />
                            )}
                        </div>

                        <button
                            className={clsx(
                                styles.button,
                                'button',
                                styles.blue
                            )}
                            onClick={handleMarkAsRead}
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

            {!currentUser ? (
                <></>
            ) : (
                <div className={styles.comments}>
                    <div className={styles.commentsHeader}>
                        <h3 className={styles.commentsTitle}>Оставьте отзыв</h3>
                    </div>
                    <CreateReview
                        placeholder='Что вы думаете о книге?'
                        onSubmit={handleReviewSubmit}
                    />
                </div>
            )}

            <div className={styles.comments}>
                <div className={styles.commentsHeader}>
                    <h3 className={styles.commentsTitle}>Отзывы</h3>
                    <ButtonAll />
                </div>
                {reviews.length > 0 ? (
                    <ul className={styles.commentsList}>
                        {reviews.map((review) => (
                            <Comment
                                key={review.createdAt + review.text}
                                user={{
                                    avatar: review.userInfo.avatarUrl,
                                    name: review.userInfo.login,
                                }}
                                date={new Date(
                                    review.createdAt
                                ).toLocaleDateString()}
                                rating={review.rating}
                                text={review.text}
                            />
                        ))}
                    </ul>
                ) : (
                    <p className={styles.noReviews}>
                        Пока нет отзывов. Будьте первым!
                    </p>
                )}
            </div>

            {similarBooks.length > 0 && (
                <div className={styles.comments}>
                    <h3 className={styles.commentsTitle}>Похожие книги</h3>
                    <ul className={styles.similarList}>
                        {similarBooks.map((similarBook) => (
                            <Book book={similarBook} />
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}
