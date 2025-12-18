import { useEffect } from 'react';
import { BooksList } from '../../components';
import {
    fetchBooksCollections,
    fetchBooksInCollection,
    selectBooksCollectionsLoading,
    selectCurrentCollectionBooks,
    selectFavoriteCollection,
    selectUser,
    useDispatch,
    useSelector,
} from '../../store';
import styles from './favorites-page.module.scss';

export const FavoritesPage = () => {
    const dispatch = useDispatch();
    const currentUser = useSelector(selectUser);
    const favoriteCollection = useSelector(selectFavoriteCollection);
    const favoriteBooks = useSelector(selectCurrentCollectionBooks);
    const loading = useSelector(selectBooksCollectionsLoading);

    useEffect(() => {
        if (!currentUser) return;

        // 1. Загружаем список подборок (чтобы найти "Избранное")
        dispatch(fetchBooksCollections({ userId: currentUser.id }));

        // 2. Если подборка "Избранное" уже известна — загружаем книги сразу
        if (favoriteCollection?.id) {
            dispatch(fetchBooksInCollection(favoriteCollection.id));
        }
    }, [dispatch, currentUser]);

    // Если подборка появилась позже — загружаем книги
    useEffect(() => {
        if (favoriteCollection?.id) {
            dispatch(fetchBooksInCollection(favoriteCollection.id));
        }
    }, [dispatch, favoriteCollection]);

    return (
        <div className={styles.container}>
            <BooksList
                title='Избранное'
                books={favoriteBooks}
                loading={loading}
            />
        </div>
    );
};
