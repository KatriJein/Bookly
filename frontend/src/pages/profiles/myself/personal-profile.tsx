import { PersonalInfo, Statistics } from '../../../components/profile';
import styles from './personal-profile.module.scss';
import clsx from 'clsx';
import { useCallback } from 'react';
import { CollectionsSmallList, EditButton } from '../../../components';
import { Comment } from '../../../components';

export function PersonalProfile() {
    const handleMenuClick = useCallback((sectionId: string) => {
        const element = document.getElementById(sectionId);
        if (element) {
            element.scrollIntoView({
                behavior: 'smooth',
                block: 'start',
            });
        }
    }, []);

    return (
        <div className={styles.personalProfile}>
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

                       <EditButton className={styles.edit}  onClick={() => {}} />
                    </ul>
                </div>
            </div>

            <div id='collections' className={styles.container}>
                <h3 className={styles.title}>Мои подборки</h3>
                <div className={styles.content}>
                    <CollectionsSmallList />
                </div>
            </div>

            <div id='reviews' className={styles.container}>
                <h3 className={styles.title}>Мои отзывы</h3>
                <div className={styles.content}>
                    <ul className={styles.comments}>
                         <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={4}
                        editable
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />

                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'John Doe',
                        }}
                        date='15.12.2023'
                        rating={5}
                        editable
                        text='Отличная книга, рекомендую всем к прочтению!'
                    />
                    <Comment
                        user={{
                            avatar: '/path/to/avatar.jpg',
                            name: 'Max Verstappen',
                        }}
                        date='06.11.2023'
                        rating={3}
                        editable
                        text='Lorem ipsum dolor sit amet consectetur adipiscing elit suscipit tincidunt sociosqu conubia parturient montes torquent.'
                    />
                    </ul>
                </div>
            </div>
        </div>
    );
}
