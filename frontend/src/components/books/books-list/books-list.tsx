import type { ShortBook } from '../../../types';
import { ButtonAll } from '../../uikit';
import { Book } from '../book/book';
import styles from './books-list.module.scss';

interface Props {
    title: string;
    books: ShortBook[];
    loading?: boolean;
    error?: string | null;
}

export function BooksList(props: Props) {
    const { title, books = [], loading, error } = props;

    if (loading) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;

    return (
        <div className={styles.books}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>
                <ButtonAll />
            </div>
            <ul className={styles.list}>
                {books.map((book) => (
                    <Book key={book.id} book={book} />
                ))}
            </ul>
        </div>
    );
}
