import styles from './collection-small.module.scss';

export function CollectionSmallMore() {
    return (
        <div className={styles.collection}>
            <a href='#' className={styles.link}>
                <div className={styles.coverMore}>
                    <div className={styles.blueBackground}></div>
                    <div className={styles.point}>
                        <p className={styles.number}>+30</p>
                    </div>
                </div>
            </a>
        </div>
    );
}
