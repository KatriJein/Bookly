export type BookCollection = {
    id: string;
    title: string;
    isStatic: boolean;
    isPublic: boolean;
    coverUrl: string | null;
    rating: number;
    ratingsCount: number;
    userInfo: {
        id: string;
        login: string;
        avatarUrl: string | null;
    };
    booksCount: number;
    userId: string;
    userRating: number | null;
};