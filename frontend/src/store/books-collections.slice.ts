import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { BookCollection } from '../types';
import {
    addBookToDynamicCollectionApi,
    addBookToStaticCollectionApi,
    createBookCollectionApi,
    deleteBookCollectionApi,
    getBooksCollectionsApi,
    getPopularCollectionsApi,
    removeBookFromDynamicCollectionApi,
    removeBookFromStaticCollectionApi,
    updateBookCollectionApi,
} from './api';

type TBooksCollectionsState = {
    collections: BookCollection[];
    popularCollections: BookCollection[];
    loading: boolean;
    error: string | null;
    currentPage: number;
    totalPages: number;
    totalItems: number;
};

const initialState: TBooksCollectionsState = {
    collections: [],
    popularCollections: [],
    loading: false,
    error: null,
    currentPage: 1,
    totalPages: 1,
    totalItems: 0,
};

export const fetchBooksCollections = createAsyncThunk(
    'booksCollections/fetch',
    async ({ userId }: { userId: string }, { rejectWithValue }) => {
        try {
            const collections = await getBooksCollectionsApi(userId);
            return { collections };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch book collections');
        }
    }
);

export const createBookCollection = createAsyncThunk(
    'booksCollections/create',
    async (
        {
            title,
            isPublic,
            userId,
        }: { title: string; isPublic: boolean; userId: string },
        { dispatch, rejectWithValue }
    ) => {
        try {
            const newCollectionId = await createBookCollectionApi(
                title,
                isPublic,
                userId
            );

            // 🆕 Сразу после создания — перезагружаем весь список подборок
            await dispatch(fetchBooksCollections({ userId })).unwrap();

            // Можно ничего не возвращать, так как состояние уже обновлено
            return newCollectionId;
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to create book collection');
        }
    }
);

// 🆕 Async Thunk: Добавить книгу в статичную коллекцию
export const addBookToStaticCollection = createAsyncThunk(
    'booksCollections/addBookToStatic',
    async (
        { collectionName, bookId }: { collectionName: string; bookId: string },
        { rejectWithValue }
    ) => {
        try {
            // Получаем токен через getState()
            // Или просто используем getToken(), как мы сделали в API
            await addBookToStaticCollectionApi(collectionName, bookId);
            return { collectionName, bookId };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to add book to static collection');
        }
    }
);

// 🆕 Async Thunk: Удалить книгу из статичной коллекции
export const removeBookFromStaticCollection = createAsyncThunk(
    'booksCollections/removeBookFromStatic',
    async (
        { collectionName, bookId }: { collectionName: string; bookId: string },
        { rejectWithValue }
    ) => {
        try {
            await removeBookFromStaticCollectionApi(collectionName, bookId);
            return { collectionName, bookId };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue(
                'Failed to remove book from static collection'
            );
        }
    }
);

// 🆕 Async Thunk: Добавить книгу в динамическую коллекцию
export const addBookToDynamicCollection = createAsyncThunk(
    'booksCollections/addBookToDynamic',
    async (
        { collectionId, bookId }: { collectionId: string; bookId: string },
        { rejectWithValue }
    ) => {
        try {
            await addBookToDynamicCollectionApi(collectionId, bookId);
            return { collectionId, bookId };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to add book to dynamic collection');
        }
    }
);

// 🆕 Async Thunk: Удалить книгу из динамической коллекции
export const removeBookFromDynamicCollection = createAsyncThunk(
    'booksCollections/removeBookFromDynamic',
    async (
        { collectionId, bookId }: { collectionId: string; bookId: string },
        { rejectWithValue }
    ) => {
        try {
            await removeBookFromDynamicCollectionApi(collectionId, bookId);
            return { collectionId, bookId };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue(
                'Failed to remove book from dynamic collection'
            );
        }
    }
);

export const updateBookCollection = createAsyncThunk(
    'booksCollections/update',
    async (
        {
            collectionId,
            title,
            isPublic,
        }: { collectionId: string; title: string; isPublic: boolean },
        { rejectWithValue }
    ) => {
        try {
            await updateBookCollectionApi(collectionId, title, isPublic);
            return { collectionId, title, isPublic };
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to update collection');
        }
    }
);

export const deleteBookCollection = createAsyncThunk(
    'booksCollections/delete',
    async ({ collectionId }: { collectionId: string }, { rejectWithValue }) => {
        try {
            await deleteBookCollectionApi(collectionId);
            return collectionId;
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to delete collection');
        }
    }
);

export const fetchPopularCollections = createAsyncThunk(
    'popularCollections/fetch',
    async (_, { rejectWithValue }) => {
        try {
            const collections = await getPopularCollectionsApi();
            return collections;
        } catch (error: unknown) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch popular collections');
        }
    }
);

export const booksCollectionsSlice = createSlice({
    name: 'booksCollections',
    initialState,
    selectors: {
        selectBooksCollections: (state) => state.collections,
        selectBooksPopularCollections: (state) => state.popularCollections,
        selectBooksCollectionsLoading: (state) => state.loading,
        selectBooksCollectionsError: (state) => state.error,
        selectBooksCollectionsPagination: (state) => ({
            currentPage: state.currentPage,
            totalPages: state.totalPages,
            totalItems: state.totalItems,
        }),
    },
    reducers: {
        clearCollections: (state) => {
            state.collections = [];
            state.currentPage = 1;
            state.totalPages = 1;
            state.totalItems = 0;
        },
        resetError: (state) => {
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchBooksCollections.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchBooksCollections.fulfilled, (state, action) => {
                state.loading = false;
                state.collections = action.payload.collections;
                // state.currentPage = action.payload.page;
                state.totalItems = action.payload.collections.length;
                state.totalPages = 1;
            })
            .addCase(fetchBooksCollections.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

            .addCase(addBookToStaticCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(addBookToStaticCollection.fulfilled, (state, action) => {
                state.loading = false;
                const { collectionName } = action.payload;
                const collection = state.collections.find(
                    (c) => c.title === collectionName
                );
                if (collection) collection.booksCount += 1;
            })
            .addCase(addBookToStaticCollection.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

            .addCase(removeBookFromStaticCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(
                removeBookFromStaticCollection.fulfilled,
                (state, action) => {
                    state.loading = false;
                    const { collectionName } = action.payload;
                    const collection = state.collections.find(
                        (c) => c.title === collectionName
                    );
                    if (collection && collection.booksCount > 0)
                        collection.booksCount -= 1;
                }
            )
            .addCase(
                removeBookFromStaticCollection.rejected,
                (state, action) => {
                    state.loading = false;
                    state.error = action.payload as string;
                }
            )

            .addCase(addBookToDynamicCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(addBookToDynamicCollection.fulfilled, (state, action) => {
                state.loading = false;
                const { collectionId } = action.payload;
                const collection = state.collections.find(
                    (c) => c.id === collectionId
                );
                if (collection) collection.booksCount += 1;
            })
            .addCase(addBookToDynamicCollection.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

            .addCase(removeBookFromDynamicCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(
                removeBookFromDynamicCollection.fulfilled,
                (state, action) => {
                    state.loading = false;
                    const { collectionId } = action.payload;
                    const collection = state.collections.find(
                        (c) => c.id === collectionId
                    );
                    if (collection && collection.booksCount > 0)
                        collection.booksCount -= 1;
                }
            )
            .addCase(
                removeBookFromDynamicCollection.rejected,
                (state, action) => {
                    state.loading = false;
                    state.error = action.payload as string;
                }
            )

            .addCase(updateBookCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(updateBookCollection.fulfilled, (state, action) => {
                state.loading = false;
                const { collectionId, title, isPublic } = action.payload;
                const collection = state.collections.find(
                    (c) => c.id === collectionId
                );
                if (collection) {
                    collection.title = title;
                    collection.isPublic = isPublic;
                }
            })
            .addCase(updateBookCollection.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

            .addCase(deleteBookCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(deleteBookCollection.fulfilled, (state, action) => {
                state.loading = false;
                const deletedId = action.payload;
                state.collections = state.collections.filter(
                    (c) => c.id !== deletedId
                );
            })
            .addCase(deleteBookCollection.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })
            .addCase(createBookCollection.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(createBookCollection.fulfilled, (state) => {
                state.loading = false;
            })
            .addCase(createBookCollection.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })
            .addCase(fetchPopularCollections.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPopularCollections.fulfilled, (state, action) => {
                state.loading = false;
                state.popularCollections = action.payload;
            })
            .addCase(fetchPopularCollections.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            });
    },
});

export const {
    selectBooksCollections,
    selectBooksCollectionsLoading,
    selectBooksCollectionsError,
    selectBooksCollectionsPagination,
    selectBooksPopularCollections
} = booksCollectionsSlice.selectors;

export default booksCollectionsSlice.reducer;
