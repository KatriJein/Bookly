import clsx from 'clsx';
import { Book } from '../../components';
import { PersonalInfo } from '../../components/profile';
import styles from './collection-page.module.scss';
import { Helmet } from 'react-helmet-async';
import type { ShortAuthor, ShortGenre } from '../../types';

const book = {
    ageRestriction: '0+',
    authors: [
        {
            id: '54411160-25fb-4080-b3d3-5fc36440f7c8',
            fullName: 'Докинз Р.',
            displayName: 'Ричард Докинз',
        } as ShortAuthor,
    ],

    genres: [
        {
            id: '0642a30e-fa33-41c1-8563-0f4edc70615f',
            name: 'Fiction',
            displayName: 'Художественная литература',
        } as ShortGenre,
    ],

    id: '019ab06e-f7d5-7b50-9be7-8054fda364da',
    isFavorite: false,
    language: 'ru',
    publishmentYear: 2014,
    rating: 5,
    thumbnail:
        'http://books.google.com/books/content?id=gkAQAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api',
    title: 'Бог как иллюзия',
    userRating: null,
};

export function CollectionPage() {
    return (
        <div className={styles.personalProfile}>
            <Helmet>
                <title>Подборка</title>
            </Helmet>
            <div className={styles.header}>
                <PersonalInfo
                    isAuthor
                    authorText='Автор подборки'
                    buttonColor='blue'
                    buttonText='В профиль'
                />
            </div>

            <div className={styles.container}>
                <div className={styles.header}>
                    <h3 className={styles.title}>Саморазвитие</h3>
                    <button className={clsx('button', styles.button)}>
                        Подписаться на подборку
                    </button>
                </div>

                <div className={styles.content}>
                    <ul className={styles.list}>
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                        <Book book={book} />
                    </ul>
                </div>
            </div>
        </div>
    );
}
