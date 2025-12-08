import type { ShortAuthor } from './short-author';
import type { ShortGenre } from './short-genre';

export type Book = {
    id: string;
    title: string;
    authors: ShortAuthor[];
    description: string;
    publishmentYear: number;
    rating: number;
    ratingsCount: number;
    publisher: string;
    pageCount: number;
    genres: ShortGenre[];
    thumbnail: string;
    language: string;
    ageRestriction: string;
    isFavorite: boolean;
    userRating: number;
    createdAt: string;
};
