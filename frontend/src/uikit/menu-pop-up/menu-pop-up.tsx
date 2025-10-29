import styles from './menu-pop-up.module.scss';
import clsx from 'clsx';
import { MenuClock, MenuDislike, MenuLike, MenuList } from '../icons';

interface MenuPopUpProps {
    className?: string;
    onClose?: () => void;
}

export function MenuPopUp(params: MenuPopUpProps) {
    const { className, onClose } = params;

    const handleItemClick = (action: string) => {
        console.log('Выбрано действие:', action);
        onClose?.();
    };

    return (
        <ul className={clsx(styles.menu, className)}>
            <li
                className={styles.item}
                onClick={() => handleItemClick('want-to-read')}
            >
                <MenuLike className={styles.icon} />
                <span>Хочу прочитать</span>
            </li>
            <li
                className={styles.item}
                onClick={() => handleItemClick('reading')}
            >
                <MenuClock className={styles.icon} />
                <span>Читаю</span>
            </li>
            <li
                className={styles.item}
                onClick={() => handleItemClick('readed')}
            >
                <MenuList className={styles.icon} />
                <span>Прочитано</span>
            </li>
            <li
                className={styles.item}
                onClick={() => handleItemClick('not-interested')}
            >
                <MenuDislike className={styles.icon} />
                <span>Не интерсено</span>
            </li>
        </ul>
    );
}
