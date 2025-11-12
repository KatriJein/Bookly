import clsx from 'clsx';
import {
    DropDownButton,
    DropDownRatingItem,
    ItemOfSort,
    SearchBar,
} from '../uikit';
import styles from './search.module.scss';
import ArrowRight from '../../assets/svg/arrow_right.svg';

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

const sortElement = {
    items: [
        {
            id: 'popular asc',
            content: ItemOfSort({
                title: 'По популярности',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'popular desc',
            content: ItemOfSort({
                title: 'По популярности',
                sortOrder: 'desc',
            }),
        },
        {
            id: 'rating asc',
            content: ItemOfSort({
                title: 'По рейтингу',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'rating desc',
            content: ItemOfSort({
                title: 'По рейтингу',
                sortOrder: 'desc',
            }),
        },
    ],
    name: 'sort',
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
                <button className={clsx('button', 'pink', styles.button)}>
                    Предложить книгу
                </button>
            </div>

            <div className={styles.filters}>
                <DropDownButton
                    text='Сортировать по'
                    listType='single'
                    color='blue'
                    {...sortElement}
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
                <button className={clsx('button', 'pink', styles.link)}>
                    <span>По вашим интересам</span>
                    <img src={ArrowRight} alt='Arrow right' />
                </button>
            </div>
        </div>
    );
}
