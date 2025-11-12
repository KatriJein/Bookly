import { Route, Routes } from 'react-router-dom';
import './App.scss';
import { MainPage, PageOfBook } from './pages';
import { Layout, LayoutMain } from './layout';
import { PersonalProfile } from './pages/profiles';

function App() {
    return (
        <Routes>
            <Route path='/' element={<LayoutMain />}>
                <Route index element={<MainPage />} />
            </Route>

            <Route path='/' element={<Layout />}>
                <Route path='/page' element={<PageOfBook />} />
                <Route path='/profile' element={<PersonalProfile />} />
            </Route>
        </Routes>
    );
}

export default App;
