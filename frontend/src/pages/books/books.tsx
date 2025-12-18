import { useEffect } from 'react';
import { BooksList, Search } from '../../components';
import {
    fetchInitialBooks,
    selectBooksLoading,
    selectHasSearched,
    selectSearchResults,
    useDispatch,
    useSelector,
} from '../../store';
import styles from './books.module.scss';

export const BooksPage = () => {
    const dispatch = useDispatch();
    const books = useSelector(selectSearchResults);
    const hasSearched = useSelector(selectHasSearched);
    const loading = useSelector(selectBooksLoading);

    useEffect(() => {
        dispatch(fetchInitialBooks());
    }, [dispatch]);

    const title = hasSearched ? 'Найденные книги' : 'Книги';

    return (
        <div className={styles.container}>
            <Search />
            <BooksList title={title} books={books} loading={loading} />
        </div>
    );
};
