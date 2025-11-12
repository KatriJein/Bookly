import clsx from 'clsx';
import { DropDownButton, DropDownRatingItem, SearchBar } from '../uikit';
import styles from './search.module.scss';

const ratingElement = {
    items: [
        {
            id: '4.5',
            content: DropDownRatingItem({
                text: '4,5 и выше',
                rating: 5,
            }),
        },
        {
            id: '4.0',
            content: DropDownRatingItem({
                text: '4,0 и выше',
                rating: 4,
            }),
        },
        {
            id: '3.0',
            content: DropDownRatingItem({
                text: '3,0 и выше',
                rating: 3,
            }),
        },
        {
            id: '2.0',
            content: DropDownRatingItem({
                text: '2,0 и выше',
                rating: 2,
            }),
        },
    ],
    name: 'rating',
};

const genres = [
    'Фантастика',
    'Драма',
    'Комедия',
    'Триллер',
    'Роман',
    'Детектив',
    'Приключения',
    'Научная литература',
    'Биография',
    'Исторический',
];

const authors = [
    'Эрих Мария Ремарк',
    'Джоан Роулинг',
    'Джеффри Робертс',
    'Джейн Остин',
    'Джордж Оруэлл',
    'Эрнест Хемингуэй',
    'Джоан Роулинг',
    'Джеффри Робертс',
    'Джейн Остин',
    'Джордж Оруэлл',
    'Эрнест Хемингуэй',
    'Джоан Роулинг',
    'Джеффри Робертс',
    'Джейн Остин',
    'Джордж Оруэлл',
    'Эрнест Хемингуэй',
];

export function Search() {
    return (
        <div className={styles.search}>
            <div className={styles.searchBar}>
                <SearchBar />
                <button className={clsx('button', styles.button)}>
                    Предложить книгу
                </button>
            </div>

            <div className={styles.filters}>
                <DropDownButton
                    text='Сортировать по'
                    listType='single'
                    color='blue'
                    {...ratingElement}
                />
                <DropDownButton
                    text='Жанр'
                    listType='multiple'
                    color='pink'
                    items={genres}
                />
                <DropDownButton
                    text='Автор'
                    listType='multiple'
                    color='pink'
                    items={authors}
                />
                <DropDownButton
                    text='Рейтинг'
                    listType='single'
                    color='pink'
                    {...ratingElement}
                />
            </div>
        </div>
    );
}
