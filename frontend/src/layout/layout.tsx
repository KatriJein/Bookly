import styles from './layout.module.scss';
import { Outlet } from 'react-router-dom';
import { Footer, Header } from '../components';

export function Layout() {
    return (
        <div className={styles.layout}>
            <Header />
            <main className={styles.main}>
                <Outlet />
            </main>
            <Footer />
        </div>
    );
}
