import { ButtonAll } from '../../uikit';
import { CollectionBig } from '../collection-big';
import styles from './collections-list.module.scss';

interface Params {
    title: string;
}

export function CollectionsList(params: Params) {
    const { title } = params;

    return (
        <div className={styles.collections}>
            <div className={styles.header}>
                <h2 className={styles.title}>{title}</h2>
                <ButtonAll />
            </div>
            <ul className={styles.list}>
                <CollectionBig />
                <CollectionBig />
                <CollectionBig />
            </ul>
        </div>
    );
}
