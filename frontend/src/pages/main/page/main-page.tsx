import { Helmet } from 'react-helmet-async';
import { MainBanner } from '../banner';
import styles from './main-page.module.scss';
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { BooksList, CollectionsList, Search } from '../../../components';
import {
    getInterestBooks,
    getRecommendedBooks,
    selectBooksError,
    selectBooksLoading,
    selectInterest,
    selectRecommended,
    useDispatch,
    useSelector,
} from '../../../store';
import { useEffect } from 'react';

export function MainPage() {
    const dispatch = useDispatch();
    const recommendedBooks = useSelector(selectRecommended);
    const interestBooks = useSelector(selectInterest);
    const loading = useSelector(selectBooksLoading);
    const error = useSelector(selectBooksError);

    useEffect(() => {
        dispatch(getRecommendedBooks({ Page: 1, Limit: 10 }));

        dispatch(getInterestBooks({ Page: 2, Limit: 10 }));
    }, [dispatch]);

    return (
        <>
            <Helmet>
                <title>Главная страница</title>
            </Helmet>

            <MainBanner />
            <div className={styles.content}>
                <Search />
                <CollectionsList title='Популярные подборки' />
                <BooksList
                    title='Вам может понравиться'
                    books={recommendedBooks}
                    loading={loading}
                    error={error}
                />
                <BooksList
                    title='Книги в ваших интересах'
                    books={interestBooks}
                    loading={loading}
                    error={error}
                />
            </div>
        </>
    );
}
