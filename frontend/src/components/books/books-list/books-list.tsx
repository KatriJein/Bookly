import { ButtonAll } from '../../uikit';
import { Book } from '../book/book';
import styles from './books-list.module.scss';

interface Params {
    title: string;
}

export function BooksList(params: Params) {
    const { title } = params;

    return (
        <div className={styles.books}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>
                <ButtonAll />
            </div>
            <ul className={styles.list}>
                <Book />
                <Book />
                <Book />
                <Book />
                <Book />
                <Book />
                <Book />
                <Book />
            </ul>
        </div>
    );
}
