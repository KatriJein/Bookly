// import { ButtonAll } from '../../uikit';
// import { CollectionBig } from '../collection-big';
import { CollectionSmall, CollectionSmallMore } from '../collections-small';
import styles from './collections-list-small.module.scss';

// interface Params {
//     title: string;
// }

export function CollectionsSmallList() {
    return (
        <ul className={styles.list}>
            <CollectionSmall />
            <CollectionSmall />
            <CollectionSmall />
            <CollectionSmallMore />
        </ul>
    );
}
