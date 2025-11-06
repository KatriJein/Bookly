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

export function Search() {
    return (
        <div className={styles.search}>
            <SearchBar />
            <DropDownButton text='Сортировать по' listType='single' color='blue' {...ratingElement} />
            <DropDownButton text='Жанр' listType='multiple' color='pink' items={genres} />
        </div>
    );
}
