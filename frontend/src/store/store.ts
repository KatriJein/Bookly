import { configureStore, combineReducers } from '@reduxjs/toolkit';
import {
    type TypedUseSelectorHook,
    useDispatch as dispatchHook,
    useSelector as selectorHook,
} from 'react-redux';
import userReducer from './user.slice';
import booksReducer from './books.slice';

export const rootReducer = combineReducers({
    user: userReducer,
    books: booksReducer,
});

const store = configureStore({
    reducer: rootReducer,
    devTools: import.meta.env?.MODE !== 'production',
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useDispatch: () => AppDispatch = dispatchHook;
export const useSelector: TypedUseSelectorHook<RootState> = selectorHook;

export default store;
