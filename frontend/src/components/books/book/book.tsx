import styles from './book.module.scss';
// import Cover from '../../../assets/images/book.png';
import Star from '../../../assets/svg/star.svg';
import Point from '../../../assets/svg/point.svg';
import Menu from '../../../assets/svg/menu.svg';
import { HeartIcon, MenuPopUp } from '../../uikit';
import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import clsx from 'clsx';
import type { ShortBook } from '../../../types';

type BookProps = {
    className?: string;
    book: ShortBook;
};

export function Book(props: BookProps) {
    const { book } = props;
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement>(null);
    const buttonRef = useRef<HTMLButtonElement>(null);
    const navigate = useNavigate();

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (
                menuRef.current &&
                !menuRef.current.contains(event.target as Node) &&
                buttonRef.current &&
                !buttonRef.current.contains(event.target as Node)
            ) {
                setIsMenuOpen(false);
            }
        }

        if (isMenuOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isMenuOpen]);

    const toggleMenu = () => {
        setIsMenuOpen(!isMenuOpen);
    };

    const closeMenu = () => {
        setIsMenuOpen(false);
    };

    const handleDetailsClick = () => {
        navigate(`/book/${book.id}`);
    };

    return (
        <div className={styles.book}>
            <img src={book.thumbnail} alt='Cover' className={styles.cover} />

            <button className={styles.like}>
                <HeartIcon
                    className={styles.icon}
                    strokeClassName={styles.stroke}
                />
            </button>

            <div className={styles.info}>
                <div className={styles.description}>
                    <p className={styles.name}>{book.title}</p>
                    <p className={styles.author}>
                        {book.authors[0].fullName}
                    </p>
                    <div className={styles.year}>
                        <span>{book.publishmentYear}</span>
                        <img src={Point} alt='Point' />
                        <div className={styles.rating}>
                            <img
                                src={Star}
                                alt='Star'
                                className={styles.star}
                            />
                            <span className={styles.value}>{book.rating}</span>
                        </div>
                    </div>
                    <ul className={styles.tags}>
                        {book.genres.map((genre) => (
                            <li className={styles.tag}>{genre.displayName}</li>
                        ))}
                    </ul>
                </div>
                <div className={styles.more} ref={menuRef}>
                    <button
                        ref={buttonRef}
                        className={styles.menu}
                        onClick={toggleMenu}
                        aria-expanded={isMenuOpen}
                        aria-label='Открыть меню'
                    >
                        <img src={Menu} alt='Menu' className={styles.icon} />
                    </button>
                    {isMenuOpen && (
                        <MenuPopUp
                            className={styles.popup}
                            onClose={closeMenu}
                        />
                    )}
                </div>
            </div>

            <button
                className={clsx('button', 'pink', styles.button)}
                onClick={handleDetailsClick}
            >
                Подробнее
            </button>
        </div>
    );
}
