// import { ButtonAll } from '../../uikit';
// import { CollectionBig } from '../collection-big';
import type { BookCollection } from '../../../types';
import { CollectionSmall, CollectionSmallMore } from '../collections-small';
import styles from './collections-list-small.module.scss';

interface CollectionsSmallListProps {
    collections: BookCollection[];
}

export function CollectionsSmallList({ collections }: CollectionsSmallListProps) {
    if (collections.length === 0) {
        return <p className={styles.empty}>У вас пока нет подборок</p>;
    }

    // Показываем максимум 3 обычных подборки + "ещё N"
    const visibleCollections = collections.slice(0, 3);
    const hiddenCount = collections.length - 3;

    return (
        <ul className={styles.list}>
            {visibleCollections.map((collection) => (
                <CollectionSmall key={collection.id} collection={collection} />
            ))}

            {/* Показываем "ещё N" только если подборок больше 3 */}
            {hiddenCount > 0 && <CollectionSmallMore count={hiddenCount} />}
        </ul>
    );
}