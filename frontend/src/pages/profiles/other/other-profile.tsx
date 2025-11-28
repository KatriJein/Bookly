import { PersonalInfo, Statistics } from '../../../components/profile';
import styles from './other-profile.module.scss';
import { CollectionsSmallList } from '../../../components';
import { Comment } from '../../../components';
import { Helmet } from 'react-helmet-async';

export function OtherProfile() {
    return (
        <div className={styles.personalProfile}>
            <Helmet>
                <title>Профиль</title>
            </Helmet>
            <div className={styles.header}>
                <PersonalInfo buttonColor='blue' buttonText='Подписаться' />
            </div>
            <div id='statistics' className={styles.container}>
                <h3 className={styles.title}>Активность</h3>
                <div className={styles.content}>
                    <Statistics
                        text='10 Просмотрено книг'
                        color1='#FFC0CB'
                        color2='#FF69B4'
                    />
                </div>
            </div>

            <div id='collections' className={styles.container}>
                <h3 className={styles.title}>Подборки</h3>
                <div className={styles.content}>
                    <CollectionsSmallList />
                </div>
            </div>

            <div id='reviews' className={styles.container}>
                <h3 className={styles.title}>Отзывы</h3>
                <div className={styles.content}>
                    <ul className={styles.comments}>
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
                    </ul>
                </div>
            </div>
        </div>
    );
}
