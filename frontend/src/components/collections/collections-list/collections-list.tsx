import type { BookCollection } from '../../../types';
import { ButtonAll } from '../../uikit';
import { CollectionBig } from '../collection-big';
import styles from './collections-list.module.scss';

interface CollectionsListProps {
    title: string;
    collections?: BookCollection[]; // ← опционально, чтобы не ломалось при загрузке
}

export function CollectionsList({ title, collections = [] }: CollectionsListProps) {
   console.log(collections);
    return (
        <div className={styles.collections}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>
                <ButtonAll />
            </div>
            <ul className={styles.list}>
                {collections.length > 0 ? (
                    collections.map((collection) => (
                        <CollectionBig
                            key={collection.id}
                            collection={collection}
                        />
                    ))
                ) 
                : (
                    // Можно показать загрузку или "нет данных", но на главной лучше первые 4
                    <>
                        <CollectionBig />
                        <CollectionBig />
                        <CollectionBig />
                        <CollectionBig />
                    </>
                )
                }
            </ul>
        </div>
    );
}
