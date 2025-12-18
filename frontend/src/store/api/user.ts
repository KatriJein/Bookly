import axios from 'axios';
import type { Review } from '../../types';
import qs from 'qs';
import { getToken } from './books-collections';

const apiUrl = 'http://localhost:8082/';

export type TLoginData = {
    login: string;
    password: string;
};

export type TRegisterData = {
    login: string;
    email: string;
    password: string;
};

export type TAuthResponse = {
    id: string;
    login: string;
    email: string;
    avatarUrl?: string;
    accessToken: string;
};

// Логин
export const loginApi = async (data: TLoginData): Promise<TAuthResponse> => {
    try {
        const response = await axios.post<TAuthResponse>(
            `${apiUrl}api/auth/login`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
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

            throw new Error('Login failed');
        }
        throw new Error('Unknown error occurred');
    }
};

// Регистрация
export const registerApi = async (
    data: TRegisterData
): Promise<TAuthResponse> => {
    try {
        const response = await axios.post<TAuthResponse>(
            `${apiUrl}api/auth/register`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
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

            throw new Error('Registration failed');
        }
        throw new Error('Unknown error occurred');
    }
};

export type TUpdateUserData = {
    login?: string;
    email?: string;
};

export type TUpdatePasswordData = {
    oldPassword: string;
    newPassword: string;
};

// Обновление информации о пользователе (PUT)
export const updateUserApi = async (
    userId: string,
    data: TUpdateUserData
): Promise<void> => {
    try {
        await axios.put(`${apiUrl}api/users/${userId}`, data, {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to update user info');
        }
        throw new Error('Unknown error occurred');
    }
};

// Обновление пароля (PATCH)
export const updatePasswordApi = async (
    userId: string,
    data: TUpdatePasswordData
): Promise<void> => {
    try {
        await axios.patch(`${apiUrl}api/users/${userId}/password`, data, {
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        });
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw new Error('Failed to update password');
        }
        throw new Error('Unknown error occurred');
    }
};

// Обновление аватарки (PATCH с multipart/form-data)
export const updateAvatarApi = async (
    userId: string,
    file: File
): Promise<string> => {
    try {
        const formData = new FormData();
        formData.append('File', file);

        const response = await axios.patch<string>(
            `${apiUrl}api/users/${userId}/avatar`,
            formData,
            {
                headers: {
                    Accept: 'text/plain',
                    'Content-Type': 'multipart/form-data',
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
            throw new Error('Failed to upload avatar');
        }
        throw new Error('Unknown error occurred');
    }
};

export const getUserReviewsApi = async (
    page = 1,
    limit = 10
): Promise<Review[]> => {
    try {
        const response = await axios.get(`${apiUrl}api/users/reviews`, {
            params: { Page: page, Limit: limit },
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getToken()}`,
            },
            withCredentials: true, // Для передачи куки/токена
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
