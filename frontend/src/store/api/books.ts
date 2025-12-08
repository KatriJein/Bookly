import axios from 'axios';
import qs from 'qs';
import type { Book, ShortBook } from '../../types';

export type TBooksData = {
    Page?: number;
    Limit?: number;
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
