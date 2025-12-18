import {
    CreateCollection,
    PersonalInfo,
    Statistics,
} from '../../../components/profile';
import styles from './personal-profile.module.scss';
import clsx from 'clsx';
import { useCallback, useEffect, useState } from 'react';
import { CollectionsSmallList, EditButton, Modal } from '../../../components';
import { Comment } from '../../../components';
import { Helmet } from 'react-helmet-async';
import {
    fetchBooksCollections,
    fetchUserReviews,
    selectBooksCollections,
    selectUser,
    selectUserReviews,
    useDispatch,
    useSelector,
} from '../../../store';

export function PersonalProfile() {
    const dispatch = useDispatch();
    const user = useSelector(selectUser);
    const reviews = useSelector(selectUserReviews);
    const collections = useSelector(selectBooksCollections);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const handleMenuClick = useCallback((sectionId: string) => {
        const element = document.getElementById(sectionId);
        if (element) {
            element.scrollIntoView({
                behavior: 'smooth',
                block: 'start',
            });
        }
    }, []);

    useEffect(() => {
        if (user?.id) {
            dispatch(fetchBooksCollections({ userId: user.id }));
            dispatch(fetchUserReviews());
        }
    }, [dispatch, user?.id]);

    const openModal = () => setIsModalOpen(true);
    const closeModal = () => setIsModalOpen(false);

    return (
        <div className={styles.personalProfile}>
            <Helmet>
                <title>Профиль</title>
            </Helmet>
            <div className={styles.header}>
                <PersonalInfo editable buttonColor='pink' />
                <ul className={styles.menu}>
                    <li onClick={() => handleMenuClick('statistics')}>
                        Моя статистика
                    </li>
                    <li onClick={() => handleMenuClick('genres')}>
                        Любимые жанры
                    </li>
                    <li onClick={() => handleMenuClick('collections')}>
                        Мои подборки
                    </li>
                    <li onClick={() => handleMenuClick('reviews')}>
                        Мои отзывы
                    </li>
                </ul>
            </div>
            <div id='statistics' className={styles.container}>
                <h3 className={styles.title}>Моя статистика</h3>
                <div className={styles.content}>
                    <Statistics
                        text='10 Просмотрено книг'
                        color1='#FFC0CB'
                        color2='#FF69B4'
                    />
                </div>
            </div>

            <div id='genres' className={styles.container}>
                <h3 className={styles.title}>Любимые жанры</h3>
                <div className={styles.content}>
                    <ul className={styles.genres}>
                        <li className={clsx('genre')}>Классика</li>
                        <li className={clsx('genre')}>Фантастика</li>
                        <li className={clsx('genre')}>Научпоп</li>
                        <li className={clsx('genre')}>Драма</li>

                        <EditButton
                            className={styles.edit}
                            onClick={() => {}}
                        />
                    </ul>
                </div>
            </div>

            <div id='collections' className={styles.container}>
                <div className={styles.containerCollection}>
                    <h3 className={styles.title}>Мои подборки</h3>
                    <button
                        className={clsx('button', 'pink', styles.button)}
                        onClick={openModal}
                    >
                        Добавить подборку
                    </button>
                </div>

                <div className={styles.content}>
                    <CollectionsSmallList collections={collections} />
                </div>
            </div>

            <div id='reviews' className={styles.container}>
                <h3 className={styles.title}>Мои отзывы</h3>
                <div className={styles.content}>
                    <ul className={styles.comments}>
                        {reviews.length > 0 ? (
                            <ul>
                                {reviews.map((review) => (
                                    <Comment
                                        user={{
                                            avatar: review.userInfo.avatarUrl,
                                            name: review.userInfo.login,
                                        }}
                                        date={review.createdAt}
                                        rating={review.rating}
                                        editable
                                        text={review.text}
                                    />
                                ))}
                            </ul>
                        ) : (
                            <p>Пока нет отзывов.</p>
                        )}
                    </ul>
                </div>
            </div>
            <Modal isOpen={isModalOpen} onClose={closeModal} width={30}>
                <CreateCollection onSuccess={closeModal} />
            </Modal>
        </div>
    );
}
