import type { ShortAuthor } from './short-author';
import type { ShortGenre } from './short-genre';

export type ShortBook = {
    id: string;
    title: string;
    authors: ShortAuthor[];
    publishmentYear: number;
    rating: number;
    genres: ShortGenre[];
    thumbnail: string;
    language: string;
    ageRestriction: string;
    isFavorite: boolean;
    userRating: number | null;
};
