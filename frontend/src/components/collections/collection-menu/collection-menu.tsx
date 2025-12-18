import styles from './collection-menu.module.scss';
import clsx from 'clsx';
import type { BookCollection } from '../../../types';

interface CollectionMenuProps {
    className?: string;
    onClose?: () => void;
    collections: BookCollection[];
    onSelect: (collection: BookCollection) => void;
}

export function CollectionMenu({
    className,
    onClose,
    collections,
    onSelect,
}: CollectionMenuProps) {
    const handleSelect = (collection: BookCollection) => {
        onSelect(collection);
        onClose?.();
    };

    return (
        <ul className={clsx(styles.menu, className)}>
            {collections.map((collection) => (
                <li
                    key={collection.id || collection.title}
                    className={styles.item}
                    onClick={() => handleSelect(collection)}
                >
                    <span>{collection.title}</span>
                    <span className={styles.bookCount}>{collection.booksCount}</span>
                </li>
            ))}
        </ul>
    );
}