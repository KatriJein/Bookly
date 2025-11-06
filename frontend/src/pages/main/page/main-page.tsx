import { Helmet } from 'react-helmet-async';
import { MainBanner } from '../banner';
import styles from './main-page.module.scss';
import { BooksList, CollectionsList, Footer, Header } from '../../../components';
// import { Search } from '../../search';
// import { RatingButton } from '../../uikit/search/drop-down-list/rating-button';

export function MainPage() {
    return (
        <>
            <Helmet>
                <title>Главная страница</title>
            </Helmet>
            <Header />
            <MainBanner />
            <div className={styles.content}>
                {/* <Search /> */}
                {/* <RatingButton /> */}
                <CollectionsList title='Популярные подборки' />
                <BooksList title='Вам может понравиться' />
                <BooksList title='Книги в ваших интересах' />
            </div>
            <Footer />
        </>
    );
}
