import clsx from 'clsx';
import { Book } from '../../components';
import { PersonalInfo } from '../../components/profile';
import styles from './collection-page.module.scss';
import { Helmet } from 'react-helmet-async';
import type { BookCollection } from '../../types';
import { useLocation, useParams } from 'react-router-dom';
import {
    fetchBooksInCollection,
    selectCurrentCollectionBooks,
    useDispatch,
    useSelector,
} from '../../store';
import { useEffect } from 'react';

// const book = {
//     ageRestriction: '0+',
//     authors: [
//         {
//             id: '54411160-25fb-4080-b3d3-5fc36440f7c8',
//             fullName: 'Докинз Р.',
//             displayName: 'Ричард Докинз',
//         } as ShortAuthor,
//     ],

//     genres: [
//         {
//             id: '0642a30e-fa33-41c1-8563-0f4edc70615f',
//             name: 'Fiction',
//             displayName: 'Художественная литература',
//         } as ShortGenre,
//     ],

//     id: '019ab06e-f7d5-7b50-9be7-8054fda364da',
//     isFavorite: false,
//     language: 'ru',
//     publishmentYear: 2014,
//     rating: 5,
//     thumbnail:
//         'http://books.google.com/books/content?id=gkAQAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api',
//     title: 'Бог как иллюзия',
//     userRating: null,
// };

export function CollectionPage() {
    const { id: collectionId } = useParams<{ id: string }>();
    const { state } = useLocation(); // ✅ Получаем state
    const dispatch = useDispatch();
    const books = useSelector(selectCurrentCollectionBooks);

    const collection: BookCollection | undefined = state?.collection;

    useEffect(() => {
        if (collectionId) {
            dispatch(fetchBooksInCollection(collectionId));
        }
    }, [dispatch, collectionId]);

    if (!collection) {
        return <div>Подборка не найдена</div>;
    }

    return (
        <div className={styles.personalProfile}>
            <Helmet>
                <title>Подборка</title>
            </Helmet>
            <div className={styles.header}>
                <PersonalInfo
                    isExternal={true}
                    externalName={collection.userInfo.login}
                    externalAvatarUrl={collection.userInfo.avatarUrl || null}
                    buttonText='В профиль'
                    buttonColor='blue'
                    onButtonClick={() => {}}
                />
            </div>

            <div className={styles.container}>
                <div className={styles.header}>
                    <h3 className={styles.title}>{collection.title}</h3>
                    <button className={clsx('button', styles.button)}>
                        Подписаться на подборку
                    </button>
                </div>

                <div className={styles.content}>
                    <ul className={styles.list}>
                        {books.map((book) => (
                            <Book key={book.id} book={book} />
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
}
