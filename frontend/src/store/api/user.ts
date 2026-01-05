import axios from 'axios';

const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:8082/';

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
