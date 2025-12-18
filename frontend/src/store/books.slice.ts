import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { ShortBook } from '../types';
import { getBooksApi, type TBooksData } from './api/books';
import type { RootState } from './store';

export interface BookFilters {
    searchQuery?: string; // Для поиска по названию
    authors?: string[]; // Массив строк для поиска по авторам
    genres?: string[]; // Массив строк для поиска по жанрам
    rating?: number; // Минимальный рейтинг (число)
    sortOrder?: string; // Порядок сортировки (строка, соответствующая BooksOrderOption)
    volumeSizePreference?: string; // Предпочтение по объему (Short, Medium, Long, VeryLong, NoMatter)
    language?: string; // Язык
    ageRestriction?: string; // Ограничение по возрасту
}

interface BooksState {
    recommendedBooks: ShortBook[];
    interestBooks: ShortBook[];
    searchResults: ShortBook[]; // Результаты поиска
    hasSearched: boolean;
    filters: BookFilters; // Текущие фильтры
    loading: boolean;
    error: string | null;
}

const initialState: BooksState = {
    recommendedBooks: [],
    interestBooks: [],
    searchResults: [], // Изначально пусто
    hasSearched: false,
    filters: {
        searchQuery: '',
        authors: [],
        genres: [],
        rating: undefined,
        sortOrder: '', // Установим значение по умолчанию
        volumeSizePreference: '',
        language: '',
        ageRestriction: '',
    },
    loading: false,
    error: null,
};

export const getRecommendedBooks = createAsyncThunk(
    'books/getRecommended',
    async (params: TBooksData) => {
        return await getBooksApi(params);
    }
);

export const getInterestBooks = createAsyncThunk(
    'books/getInterest',
    async (params: TBooksData) => {
        return await getBooksApi(params);
    }
);

export const updateSearchQuery = (query: string) => ({
    type: 'books/updateSearchQuery',
    payload: query,
});

export const updateAuthorsFilter = (authors: string[]) => ({
    type: 'books/updateAuthorsFilter',
    payload: authors,
});

export const updateGenresFilter = (genres: string[]) => ({
    type: 'books/updateGenresFilter',
    payload: genres,
});

export const updateRatingFilter = (rating: number | undefined) => ({
    type: 'books/updateRatingFilter',
    payload: rating,
});

export const updateSortOrder = (order: string) => ({
    type: 'books/updateSortOrder',
    payload: order,
});

export const updateVolumeSizePreference = (preference: string) => ({
    type: 'books/updateVolumeSizePreference',
    payload: preference,
});

export const updateLanguage = (language: string) => ({
    type: 'books/updateLanguage',
    payload: language,
});

export const updateAgeRestriction = (restriction: string) => ({
    type: 'books/updateAgeRestriction',
    payload: restriction,
});

export const fetchInitialBooks = createAsyncThunk(
    'books/fetchInitialBooks',
    async () => {
        // Запрашиваем 28 книг без фильтров
        return await getBooksApi({ Limit: 30 });
    }
);

// --- Асинхронный экшен для поиска книг с фильтрами ---
export const searchBooks = createAsyncThunk(
    'books/searchBooks',
    async (_, { getState }) => {
        const state = getState() as RootState; // Тип RootState нужно будет определить или использовать any
        const { filters } = state.books;

        // Подготовка параметров запроса согласно Swagger
        const params: TBooksData & {
            SearchByTitle?: string;
            SearchByAuthors?: string[];
            SearchByGenres?: string[];
            SearchByRating?: number;
            BooksOrderOption?: string;
            SearchByVolumeSizePreference?: string;
            Language?: string;
            AgeRestriction?: string;
        } = {};

        if (filters.searchQuery && filters.searchQuery.trim()) {
            params.SearchByTitle = filters.searchQuery.trim();
        }

        if (filters.authors && filters.authors.length > 0) {
            params.SearchByAuthors = filters.authors;
        }

        if (filters.genres && filters.genres.length > 0) {
            params.SearchByGenres = filters.genres;
        }

        if (filters.rating !== undefined) {
            params.SearchByRating = filters.rating;
        }

        if (filters.sortOrder) {
            params.BooksOrderOption = filters.sortOrder;
        }

        if (filters.volumeSizePreference) {
            params.SearchByVolumeSizePreference = filters.volumeSizePreference;
        }

        if (filters.language) {
            params.Language = filters.language;
        }

        if (filters.ageRestriction) {
            params.AgeRestriction = filters.ageRestriction;
        }

        // Можно добавить Page и Limit, если нужны пагинация
        // params.Page = 1;
        // params.Limit = 10;

        return await getBooksApi(params);
    }
);

const booksSlice = createSlice({
    name: 'books',
    initialState,
    selectors: {
        selectRecommended: (state) => state.recommendedBooks,
        selectInterest: (state) => state.interestBooks,
        selectBooksError: (state) => state.error,
        selectBooksLoading: (state) => state.loading,
        selectSearchResults: (state) => state.searchResults,
        selectFilters: (state) => state.filters,
        selectHasSearched: (state) => state.hasSearched,
    },
    reducers: {
        // Редюсеры для обновления фильтров
        updateSearchQuery: (state, action) => {
            state.filters.searchQuery = action.payload;
        },
        updateAuthorsFilter: (state, action) => {
            state.filters.authors = action.payload;
        },
        updateGenresFilter: (state, action) => {
            state.filters.genres = action.payload;
        },
        updateRatingFilter: (state, action) => {
            state.filters.rating = action.payload;
        },
        updateSortOrder: (state, action) => {
            state.filters.sortOrder = action.payload;
        },
        updateVolumeSizePreference: (state, action) => {
            state.filters.volumeSizePreference = action.payload;
        },
        updateLanguage: (state, action) => {
            state.filters.language = action.payload;
        },
        updateAgeRestriction: (state, action) => {
            state.filters.ageRestriction = action.payload;
        },
    },
    extraReducers: (builder) => {
        builder
            // Recommended books
            .addCase(getRecommendedBooks.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(getRecommendedBooks.fulfilled, (state, action) => {
                state.loading = false;
                state.recommendedBooks = action.payload;
            })
            .addCase(getRecommendedBooks.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Ошибка загрузки';
            })
            // Interest books
            .addCase(getInterestBooks.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(getInterestBooks.fulfilled, (state, action) => {
                state.loading = false;
                state.interestBooks = action.payload;
            })
            .addCase(getInterestBooks.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Ошибка загрузки';
            })
            .addCase(searchBooks.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(searchBooks.fulfilled, (state, action) => {
                state.loading = false;
                state.searchResults = action.payload;
                state.hasSearched = true; // Сохраняем результаты поиска
            })
            .addCase(searchBooks.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Ошибка поиска книг';
            })
            .addCase(fetchInitialBooks.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchInitialBooks.fulfilled, (state, action) => {
                state.loading = false;
                state.searchResults = action.payload;
            })
            .addCase(fetchInitialBooks.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Ошибка поиска книг';
            });
    },
});

export const {
    selectBooksLoading,
    selectBooksError,
    selectInterest,
    selectRecommended,
    selectSearchResults, // Новый селектор для результатов поиска
    selectFilters,
    selectHasSearched,
} = booksSlice.selectors;

export default booksSlice.reducer;
