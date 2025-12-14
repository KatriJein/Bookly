import axios from 'axios';
import type { BookCollection } from '../../types';

const apiUrl = 'http://localhost:8082/';

export const getBooksCollectionsApi = async (
    userId: string,
    page?: number,
    limit?: number
): Promise<BookCollection[]> => {
    try {
        const params: Record<string, number> = {};
        if (page !== undefined) params.Page = page;
        if (limit !== undefined) params.Limit = limit;

        const response = await axios.get<BookCollection[]>(
            `${apiUrl}api/book-collections/${userId}`,
            {
                params,
                headers: {
                    Accept: 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to fetch book collections');
        }
        throw new Error('Unknown error occurred');
    }
};

export const createBookCollectionApi = async (
    title: string,
    isPublic: boolean,
    userId: string
): Promise<string> => {
    try {
        const response = await axios.post<string>(
            `${apiUrl}api/book-collections`,
            {
                title,
                isPublic,
                userId,
            },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${getToken()}`,
                },
            }
        );

        return response.data; // Возвращает ID новой коллекции
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to create book collection');
        }
        throw new Error('Unknown error occurred');
    }
};

// Добавить книгу в статичную коллекцию
export const addBookToStaticCollectionApi = async (
    collectionName: string,
    bookId: string
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/book-collections/static/add`,
            {
                collectionName,
                bookId,
            },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${getToken()}`, // ← получаем токен здесь
                },
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to add book to static collection');
        }
        throw new Error('Unknown error occurred');
    }
};

// Удалить книгу из статичной коллекции
export const removeBookFromStaticCollectionApi = async (
    collectionName: string,
    bookId: string
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/book-collections/static/remove`,
            {
                collectionName,
                bookId,
            },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${getToken()}`, // ← получаем токен здесь
                },
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to remove book from static collection');
        }
        throw new Error('Unknown error occurred');
    }
};

// Добавить книгу в динамическую коллекцию (по ID)
export const addBookToDynamicCollectionApi = async (
    collectionId: string,
    bookId: string
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/book-collections/add`,
            {
                collectionIds: [collectionId],
                bookId,
            },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${getToken()}`, // ← получаем токен здесь
                },
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to add book to dynamic collection');
        }
        throw new Error('Unknown error occurred');
    }
};

// Удалить книгу из динамической коллекции (по ID)
export const removeBookFromDynamicCollectionApi = async (
    collectionId: string,
    bookId: string
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/book-collections/remove`,
            {
                collectionId,
                bookId,
            },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${getToken()}`, // ← получаем токен здесь
                },
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to remove book from dynamic collection');
        }
        throw new Error('Unknown error occurred');
    }
};

export const updateBookCollectionApi = async (
    collectionId: string,
    title: string,
    isPublic: boolean
): Promise<void> => {
    await axios.put(
        `${apiUrl}api/book-collections/${collectionId}`,
        { title, isPublic },
        {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getToken()}`,
            },
        }
    );
};

// 🆕 Удаление подборки
export const deleteBookCollectionApi = async (
    collectionId: string
): Promise<void> => {
    await axios.delete(`${apiUrl}api/book-collections/${collectionId}`, {
        headers: {
            Accept: 'application/json',
            Authorization: `Bearer ${getToken()}`,
        },
    });
};

export const getPopularCollectionsApi = async (): Promise<BookCollection[]> => {
    try {
        const response = await axios.get<BookCollection[]>(
            `${apiUrl}api/book-collections`,
            {
                headers: {
                    Accept: 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to fetch popular collections');
        }
        throw new Error('Unknown error occurred');
    }
};

// 🆕 Функция для получения токена
export const getToken = (): string => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
        throw new Error('No access token found');
    }
    return token;
};
