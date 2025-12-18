import styles from './menu-pop-up.module.scss';
import clsx from 'clsx';
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { MenuClock, MenuDislike, MenuLike, MenuList } from '../icons';

// eslint-disable-next-line react-refresh/only-export-components
export const STATIC_COLLECTIONS = {
    WANT_TO_READ: 'Хочу прочитать',
    READING: 'Читаю',
    READED: 'Прочитано',
} as const;

interface MenuPopUpProps {
    className?: string;
    onClose?: () => void;
    onAddToCollection: (collectionName: string) => void;
}

export function MenuPopUp(params: MenuPopUpProps) {
    const { className, onClose, onAddToCollection } = params;

    const handleSelect = (collectionName: string) => {
        onAddToCollection(collectionName);
        onClose?.();
    };

    return (
        <ul className={clsx(styles.menu, className)}>
           <li
                className={styles.item}
                onClick={() => handleSelect(STATIC_COLLECTIONS.WANT_TO_READ)}
            >
                <MenuLike className={styles.icon} />
                <span>{STATIC_COLLECTIONS.WANT_TO_READ}</span>
            </li>
           <li
                className={styles.item}
                onClick={() => handleSelect(STATIC_COLLECTIONS.READING)}
            >
                <MenuClock className={styles.icon} />
                <span>{STATIC_COLLECTIONS.READING}</span>
            </li>
            <li
                className={styles.item}
                onClick={() => handleSelect(STATIC_COLLECTIONS.READED)}
            >
                <MenuList className={styles.icon} />
                <span>{STATIC_COLLECTIONS.READED}</span>
            </li>
            {/* <li
                className={styles.item}
                onClick={() => handleItemClick('not-interested')}
            >
                <MenuDislike className={styles.icon} />
                <span>Не интерсено</span>
            </li> */}
        </ul>
    );
}
