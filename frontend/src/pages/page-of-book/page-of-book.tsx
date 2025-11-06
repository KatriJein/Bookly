import styles from './page-of-book.module.scss';
import Cover from '../../assets/images/book.png';
import Chat from './../../assets/svg/icon-chat.svg';
import List from '../../assets/svg/list-check.svg';
import {
    BookRating,
    ListPoint,
    Comment,
    ButtonAll,
} from '../../components';
import clsx from 'clsx';

export function PageOfBook() {
    return (
        <div className={styles.pageOfBook}>
            <div className={styles.content}>
                <img src={Cover} alt='Cover' className={styles.cover} />
                <div className={styles.info}>
                    <div className={styles.description}>
                        <h2 className={styles.title}>Жизнь взаймы</h2>
                        <p className={styles.author}>Эрих Мария Ремарк</p>
                        <ListPoint
                            className={styles.list}
                            items={[
                                '2025',
                                'Издательство "Просвещение"',
                                '500 стр.',
                            ]}
                        />
                        <div className={styles.tags}>
                            <span className={styles.age}>12+</span>
                            <span className={styles.genre}>Классика</span>
                            <span className={styles.genre}>Фантастика</span>
                            <span className={styles.genre}>Научпоп</span>
                            <span className={styles.genre}>Драма</span>
                        </div>
                        <BookRating className={styles.rating} rating={3.2} />
                        <p className={styles.descriptionText}>
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги
                            Описание книги Описание книги Описание книги{' '}
                        </p>
                    </div>
                    <div className={styles.buttons}>
                        <button
                            className={clsx(
                                styles.button,
                                'button',
                                styles.pink
                            )}
                        >
                            <img
                                src={Chat}
                                alt='Chat'
                                className={styles.icon}
                            />
                            Добавить в подборку
                        </button>
                        <button
                            className={clsx(
                                styles.button,
                                'button',
                                styles.blue
                            )}
                        >
                            <img
                                src={List}
                                alt='List'
                                className={styles.icon}
                            />
                            Отметить прочитанной
                        </button>
                    </div>
                </div>
            </div>
            <div className={styles.comments}>
                <div className={styles.commentsHeader}>
                    <h3 className={styles.commentsTitle}>Отзывы</h3>
                    <ButtonAll />
                </div>

                <ul className={styles.commentsList}>
                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={4}
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />

                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'John Doe',
                        }}
                        date='15.12.2023'
                        rating={5}
                        text='Отличная книга, рекомендую всем к прочтению!'
                    />
                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={3}
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />

                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'John Doe',
                        }}
                        date='15.12.2023'
                        rating={2}
                        text='Отличная книга, рекомендую всем к прочтению!'
                    />
                </ul>
            </div>
        </div>
    );
}
