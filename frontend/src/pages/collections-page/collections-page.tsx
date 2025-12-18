import { CollectionBig } from '../../components';
import { selectBooksCollections, useSelector } from '../../store';
import styles from './collections-page.module.scss';

export const MyCollectionsPage = () => {
  const collections = useSelector(selectBooksCollections);
  
    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Мои подборки</h1>
            <ul className={styles.list}>
                {collections.map((collection) => (
                    <CollectionBig
                        key={collection.id}
                        collection={collection}
                    />
                ))}
            </ul>
        </div>
    );
};
