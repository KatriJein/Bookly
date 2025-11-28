import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { loginApi, registerApi } from './api';
import type { User } from '../types';

type TUserState = {
    user: User | null;
    error: string | null | undefined;
    isAuthenticated: boolean;
    isAuthChecked: boolean;
    isLoading: boolean;
};

export const initialState: TUserState = {
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
