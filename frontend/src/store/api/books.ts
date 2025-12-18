import axios from 'axios';
import qs from 'qs';
import type { Book, Review, ShortBook } from '../../types';
import { getToken } from './books-collections';

export type TBooksData = {
    Page?: number;
    Limit?: number;
    SearchByTitle?: string;
    SearchByAuthors?: string[];
    SearchByGenres?: string[];
    SearchByRating?: number;
    BooksOrderOption?: string;
    SearchByVolumeSizePreference?: string;
    Language?: string;
    AgeRestriction?: string;
};

export type TBooksResponse = ShortBook[];

const apiUrl = 'http://localhost:8082/';

// Получение списка книг
export const getBooksApi = async (
    data: TBooksData
): Promise<TBooksResponse> => {
    try {
        const params = Object.fromEntries(
            Object.entries(data).filter(
                // eslint-disable-next-line @typescript-eslint/no-unused-vars
                ([_, value]) =>
                    value !== undefined &&
                    value !== null &&
                    // value !== '' &&
                    value !== 0 &&
                    (!Array.isArray(value) || value.length > 0)
            )
        );

        const response = await axios.get<TBooksResponse>(`${apiUrl}api/books`, {
            params: params,
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });

        console.log(response.data, 'getBooksApi');
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting books going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Получение книги
export const getBookByIdApi = async (id: string): Promise<Book> => {
    try {
        const response = await axios.get<Book>(`${apiUrl}api/books/${id}`, {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting book going wrong');
        }

        throw new Error('Unknown error occurred');
    }
};

export const getBooksInCollectionApi = async (
    collectionId: string // UUID подборки
): Promise<TBooksResponse> => {
    try {
        const response = await axios.get<TBooksResponse>(`${apiUrl}api/books`, {
            params: {
                SearchInBookCollection: collectionId, // Передаём UUID подборки
            },
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });

        console.log(response.data, 'getBooksInCollectionApi');
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting books in collection going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

export interface CreateReviewRequest {
    text: string;
    rating: number;
    bookId: string;
}

export const createReviewApi = async (
    data: CreateReviewRequest
): Promise<void> => {
    try {
        const response = await axios.post(`${apiUrl}api/reviews/create/v2`, data, {
            headers: {
                'Content-Type': 'application/json',
                // Если нужен авторизационный токен:
                Authorization: `Bearer ${getToken()}`,
            },
            withCredentials: true,
        });

        console.log('Review created:', response.data);
        return response.data; // В зависимости от того, что возвращает сервер
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Failed to create review');
        }
        throw new Error('Unknown error occurred');
    }
};

export const getBookReviewsApi = async (
    bookId: string,
    page = 1,
    limit = 10
): Promise<Review[]> => {
    try {
        const response = await axios.get(`${apiUrl}api/books/${bookId}/reviews`, {
            params: { Page: page, Limit: limit },
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Ошибка при получении отзывов'
            );
        }
        throw new Error('Неизвестная ошибка');
    }
};

// --- Новый: получить похожие книги ---
export const getSimilarBooksApi = async (
    bookId: string,
    page = 1,
    limit = 5
): Promise<ShortBook[]> => {
    try {
        const response = await axios.get(`${apiUrl}api/books/${bookId}/similar`, {
            params: { Page: page, Limit: limit },
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Ошибка при получении похожих книг'
            );
        }
        throw new Error('Неизвестная ошибка');
    }
};