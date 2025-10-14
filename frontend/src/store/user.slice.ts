import { createSlice } from '@reduxjs/toolkit';

type TUserState = {
    user: null;
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
    reducers: {},
});

export const {
    selectUser,
    selectIsAuthenticated,
    selectIsAuthChecked,
    selectUserError,
    selectUserLoading,
} = userSlice.selectors;

export default userSlice.reducer;
