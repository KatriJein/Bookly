import { Helmet } from 'react-helmet-async';
import { Header } from '../../header';
import { MainBanner } from '../banner';
import styles from './main-page.module.scss';
import { CollectionsList } from '../../collections';
import { BooksList } from '../../books/books-list';
import { Footer } from '../../footer';

export function MainPage() {
    return (
        <>
            <Helmet>
                <title>Главная страница</title>
            </Helmet>
            <Header />
            <MainBanner />
            <div className={styles.content}>
                <CollectionsList title='Популярные подборки' />
                <BooksList title='Вам может понравиться' />
                <BooksList title='Книги в ваших интересах' />
            </div>
            <Footer />
        </>
    );
}
