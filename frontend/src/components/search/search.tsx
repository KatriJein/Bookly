import clsx from 'clsx';
import {
    DropDownButton,
    DropDownRatingItem,
    ItemOfSort,
    SearchBar,
} from '../uikit';
import styles from './search.module.scss';
import ArrowRight from '../../assets/svg/arrow_right.svg';
import {
    searchBooks,
    selectFilters,
    updateAgeRestriction,
    updateAuthorsFilter,
    updateGenresFilter,
    updateRatingFilter,
    updateSearchQuery,
    updateSortOrder,
    updateVolumeSizePreference,
    useDispatch,
    useSelector,
} from '../../store';

const ratingElement = {
    items: [
        {
            id: '4.5',
            content: DropDownRatingItem({
                text: '4,5 и выше',
                rating: 4.5,
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
            id: 'ByPopularityAscending',
            content: ItemOfSort({
                title: 'По популярности',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'ByPopularityDescending',
            content: ItemOfSort({
                title: 'По популярности',
                sortOrder: 'desc',
            }),
        },
        {
            id: 'ByRatingAscending',
            content: ItemOfSort({
                title: 'По рейтингу',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'ByRatingDescending',
            content: ItemOfSort({
                title: 'По рейтингу',
                sortOrder: 'desc',
            }),
        },
        {
            id: 'ByAlphabetAscending',
            content: ItemOfSort({
                title: 'По алфавиту',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'ByAlphabetDescending',
            content: ItemOfSort({
                title: 'По алфавиту',
                sortOrder: 'desc',
            }),
        },
        {
            id: 'ByPublicationDateAscending',
            content: ItemOfSort({
                title: 'По дате публикации',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'ByPublicationDateDescending',
            content: ItemOfSort({
                title: 'По дате публикации',
                sortOrder: 'desc',
            }),
        },
        {
            id: 'ByNumberOfPagesAscending',
            content: ItemOfSort({
                title: 'По количеству страниц',
                sortOrder: 'asc',
            }),
        },
        {
            id: 'ByNumberOfPagesDescending',
            content: ItemOfSort({
                title: 'По количеству страниц',
                sortOrder: 'desc',
            }),
        },
    ],
    name: 'sort',
};

const volumeSizePreferences = [
    { id: 'Short', label: 'Короткие' },
    { id: 'Medium', label: 'Средние' },
    { id: 'Long', label: 'Длинные' },
    { id: 'VeryLong', label: 'Очень длинные' },
    { id: 'NoMatter', label: 'Не важно' },
];

// --- Список вариантов возрастного ограничения ---
const ageRestrictions = [
    { id: 'Everyone', label: 'Для всех' },
    { id: 'Children', label: '6+' },
    { id: 'Teen', label: '12+' },
    { id: 'YoungAdult', label: '16+' },
    { id: 'Mature', label: '18+' },
    { id: 'Unspecified', label: 'Не важно' },
];

const volumeElement = {
    items: volumeSizePreferences.map((pref) => ({
        id: pref.id,
        content: pref.label,
    })),
    name: 'volume',
};

// --- Элемент для возраста ---
const ageElement = {
    items: ageRestrictions.map((restr) => ({
        id: restr.id,
        content: restr.label,
    })),
    name: 'age',
};

const genres = [
    'Фантастика',
    'Драма',
    'Комедия',
    'Триллер',
    'Роман',
    'Детектив',
    'Саморазвитие',
    'Приключения',
    'Научная литература',
    'Биография',
    'Исторический',
];

const authors = [
    'Ричард Докинз',
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
    const dispatch = useDispatch();
    const filters = useSelector(selectFilters);

    const handleSearchClick = () => {
        dispatch(searchBooks());
    };

    // Обновление поискового запроса
    const handleSearchChange = (query: string) => {
        dispatch(updateSearchQuery(query));
    };

    // Обновление сортировки
    const handleSortApply = (selectedId: string | null) => {
        if (selectedId) {
            dispatch(updateSortOrder(selectedId));
        }
    };

    // Обновление жанров
    const handleGenresApply = (selectedItems: string[]) => {
        dispatch(updateGenresFilter(selectedItems));
    };

    // Обновление авторов
    const handleAuthorsApply = (selectedItems: string[]) => {
        dispatch(updateAuthorsFilter(selectedItems));
    };

    // Обновление рейтинга
    const handleRatingApply = (selectedId: string | null) => {
        if (selectedId) {
            const rating = parseFloat(selectedId);
            dispatch(updateRatingFilter(rating));
        }
    };

    // Обновление предпочтения по объему
    const handleVolumeApply = (selectedId: string | null) => {
        if (selectedId) {
            dispatch(updateVolumeSizePreference(selectedId));
        }
    };

    // Обновление возрастного ограничения
    const handleAgeApply = (selectedId: string | null) => {
        if (selectedId) {
            dispatch(updateAgeRestriction(selectedId));
        }
    };

    return (
        <div className={styles.search}>
            <div className={styles.searchBar}>
                <SearchBar
                    onSearch={handleSearchChange}
                    // onFindClick={handleSearchClick}
                    placeholder='Поиск по названию'
                />
                <button
                    onClick={handleSearchClick}
                    className={clsx('button', 'pink', styles.button)}
                >
                    Найти
                </button>
            </div>

            <div className={styles.filters}>
                <DropDownButton
                    text='Сортировать по'
                    listType='single'
                    color='blue'
                    {...sortElement}
                    onApply={(selected: unknown) => {
                        handleSortApply(selected as string | null);
                    }}
                    initialSelection={filters.sortOrder} // Устанавливаем начальное значение
                />
                <DropDownButton
                    text='Жанр'
                    listType='multiple'
                    color='pink'
                    items={genres}
                    onApply={(selected: unknown) => {
                        handleGenresApply(selected as string[]);
                    }}
                    initialSelection={filters.genres} // Устанавливаем начальное значение
                />
                <DropDownButton
                    text='Автор'
                    listType='multiple'
                    color='pink'
                    items={authors}
                    onApply={(selected: unknown) => {
                        handleAuthorsApply(selected as string[]);
                    }}
                    initialSelection={filters.authors} // Устанавливаем начальное значение
                />
                <DropDownButton
                    text='Рейтинг'
                    listType='single'
                    color='pink'
                    {...ratingElement}
                    onApply={(selected: unknown) => {
                        handleRatingApply(selected as string | null);
                    }}
                    initialSelection={
                        filters.rating ? filters.rating.toString() : null
                    } // Устанавливаем начальное значение
                />
                <DropDownButton
                    text='Объем'
                    listType='single'
                    color='pink'
                    {...volumeElement}
                    onApply={(selected: unknown) => {
                        handleVolumeApply(selected as string | null);
                    }}
                    initialSelection={filters.volumeSizePreference}
                />
                <DropDownButton
                    text='Возраст'
                    listType='single'
                    color='pink'
                    {...ageElement}
                    onApply={(selected: unknown) => {
                        handleAgeApply(selected as string | null);
                    }}
                    initialSelection={filters.ageRestriction}
                />
                <button className={clsx('button', 'pink', styles.link)}>
                    <span>По вашим интересам</span>
                    <img src={ArrowRight} alt='Arrow right' />
                </button>
            </div>
        </div>
    );
}
