import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    loginApi,
    registerApi,
    updateAvatarApi,
    updatePasswordApi,
    updateUserApi,
    type TUpdatePasswordData,
    type TUpdateUserData,
} from './api';
import type { User } from '../types';

type TUserState = {
    user: User | null;
    error: string | null | undefined;
    isAuthenticated: boolean;
    isAuthChecked: boolean;
    isLoading: boolean;
};

const initialState: TUserState = {
    user: null,
    error: null,
    isAuthenticated: false,
    isAuthChecked: false,
    isLoading: false,
};

// Async Thunks
export const login = createAsyncThunk(
    'user/login',
    async (
        credentials: { login: string; password: string },
        { rejectWithValue }
    ) => {
        try {
            const response = await loginApi(credentials);
            localStorage.setItem('accessToken', response.accessToken);
            return response;
        } catch (error: unknown) {
            // Типизированная проверка ошибки
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Login failed');
        }
    }
);

export const register = createAsyncThunk(
    'user/register',
    async (
        userData: { login: string; email: string; password: string },
        { rejectWithValue }
    ) => {
        try {
            const response = await registerApi(userData);
            localStorage.setItem('accessToken', response.accessToken);
            return response;
        } catch (error: unknown) {
            // Типизированная проверка ошибки
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Login failed');
        }
    }
);

export const updateUser = createAsyncThunk(
    'user/updateUser',
    async (
        { userId, data }: { userId: string; data: TUpdateUserData },
        { rejectWithValue }
    ) => {
        try {
            await updateUserApi(userId, data);
            return data;
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to update user info');
        }
    }
);

export const updatePassword = createAsyncThunk(
    'user/updatePassword',
    async (
        { userId, data }: { userId: string; data: TUpdatePasswordData },
        { rejectWithValue }
    ) => {
        try {
            await updatePasswordApi(userId, data);

            return { success: true };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to update password');
        }
    }
);

export const updateAvatar = createAsyncThunk(
    'user/updateAvatar',
    async (
        { userId, file }: { userId: string; file: File },
        { rejectWithValue }
    ) => {
        try {
            const avatarUrl = await updateAvatarApi(userId, file);

            return avatarUrl;
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to upload avatar');
        }
    }
);

// Slice
export const userSlice = createSlice({
    name: 'user',
    initialState,
    selectors: {
        selectUser: (state) => state.user,
        selectIsAuthenticated: (state) => state.isAuthenticated,
        selectIsAuthChecked: (state) => state.isAuthChecked,
        selectUserError: (state) => state.error,
        selectUserLoading: (state) => state.isLoading,
    },
    reducers: {
        logout: (state) => {
            state.user = null;
            state.isAuthenticated = false;
            state.error = null;
            localStorage.removeItem('accessToken');
        },
        clearError: (state) => {
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        builder
            // Авторизация
            .addCase(login.pending, (state) => {
                state.isLoading = true;
                state.error = null;
                state.isAuthChecked = false;
            })
            .addCase(login.fulfilled, (state, action) => {
                state.isLoading = false;
                state.user = action.payload;
                state.isAuthenticated = true;
                state.isAuthChecked = true;
            })
            .addCase(login.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
                state.isAuthenticated = false;
                state.isAuthChecked = true;
            })
            .addCase(register.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(register.fulfilled, (state, action) => {
                state.isLoading = false;
                state.user = action.payload;
                state.isAuthenticated = true;
                state.isAuthChecked = true;
            })
            .addCase(register.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
                state.isAuthenticated = false;
                state.isAuthChecked = true;
            })

            // Обновление пользователя
            .addCase(updateUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updateUser.fulfilled, (state, action) => {
                state.isLoading = false;
                if (state.user) {
                    state.user.login = action.payload.login || state.user.login;
                    state.user.email = action.payload.email || state.user.email;
                }
            })
            .addCase(updateUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // Обновление пароля
            .addCase(updatePassword.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updatePassword.fulfilled, (state) => {
                state.isLoading = false;
            })
            .addCase(updatePassword.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // Обновление аватарки
            .addCase(updateAvatar.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(updateAvatar.fulfilled, (state, action) => {
                state.isLoading = false;
                if (state.user) {
                    state.user.avatarUrl = action.payload;
                }
            })
            .addCase(updateAvatar.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            });
    },
});

export const {
    selectUser,
    selectIsAuthenticated,
    selectIsAuthChecked,
    selectUserError,
    selectUserLoading,
} = userSlice.selectors;

export default userSlice.reducer;
