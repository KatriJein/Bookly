import styles from './layout.module.scss';
import { Outlet, useLocation } from 'react-router-dom';
import { Footer, Header } from '../components';
import { useLayoutEffect } from 'react';

export function Layout() {
    const { pathname } = useLocation();

    useLayoutEffect(() => {
        window.scrollTo(0, 0);
    }, [pathname]);

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
