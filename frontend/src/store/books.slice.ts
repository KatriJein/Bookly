import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { ShortBook } from '../types';
import { getBooksApi, type TBooksData } from './api/books';

interface BooksState {
    recommendedBooks: ShortBook[];
    interestBooks: ShortBook[];
    loading: boolean;
    error: string | null;
}

const initialState: BooksState = {
    recommendedBooks: [],
    interestBooks: [],
    loading: false,
    error: null,
};

export const getRecommendedBooks = createAsyncThunk(
    'books/getRecommended',
    async (params: TBooksData) => {
        const response = await getBooksApi(params);
        console.log(response, 'response');
       
        return response;
    }
);

export const getInterestBooks = createAsyncThunk(
    'books/getInterest',
    async (params: TBooksData) => {
        const response = await getBooksApi(params);
        console.log(response, 'response');
      
        return response;
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
    },
    reducers: {},
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
            });
    },
});

export const {
    selectBooksLoading,
    selectBooksError,
    selectInterest,
    selectRecommended,
} = booksSlice.selectors;

export default booksSlice.reducer;
