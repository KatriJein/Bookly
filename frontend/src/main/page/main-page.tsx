import { Helmet } from 'react-helmet-async';
import { Header } from '../../header';
import { MainBanner } from '../banner';

export function MainPage() {
    return (
        <>
            <Helmet>
                <title>Главная страница</title>
            </Helmet>
            <Header />
            <MainBanner />
        </>
    );
}
