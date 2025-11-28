import { PersonalInfo } from '../../../components/profile';
import styles from './author-profile.module.scss';
import clsx from 'clsx';
import { Book } from '../../../components';
import { Helmet } from 'react-helmet-async';

export function AuthorProfile() {
    return (
        <div className={styles.personalProfile}>
            <Helmet>
                <title>Профиль автора</title>
            </Helmet>
            <div className={styles.header}>
                <PersonalInfo
                    isAuthor
                    buttonColor='blue'
                    buttonText='Подписаться'
                />
            </div>

            <div className={styles.container}>
                <h3 className={styles.title}>Жанры</h3>
                <div className={styles.content}>
                    <ul className={styles.genres}>
                        <li className={clsx('genre')}>Классика</li>
                        <li className={clsx('genre')}>Фантастика</li>
                        <li className={clsx('genre')}>Научпоп</li>
                        <li className={clsx('genre')}>Драма</li>
                    </ul>
                </div>
            </div>

            <div className={styles.container}>
                <h3 className={styles.title}>Книги</h3>
                <div className={styles.content}>
                    {/* <ul className={styles.list}>
                        <Book />
                        <Book />
                        <Book />
                        <Book />
                        <Book />
                        <Book />
                        <Book />
                        <Book />
                    </ul> */}
                </div>
            </div>
        </div>
    );
}
