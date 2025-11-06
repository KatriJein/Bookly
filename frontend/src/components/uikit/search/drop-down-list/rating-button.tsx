import { DropDownMultiple } from './multiple';
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { DropDownRatingItem, DropDownSingle } from './single';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
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

export function RatingButton() {
    const handleApplyGenres = (selectedGenres: string[]) => {
        console.log('Выбранные жанры:', selectedGenres);
    };

    return (
        // <div>
        //     <DropDownSingle {...ratingElement} />
        // </div>
        <DropDownMultiple
            items={genres}
            onApply={handleApplyGenres}
            searchPlaceholder='Поиск жанров...'
            applyText='Применить'
        />
    );
}
