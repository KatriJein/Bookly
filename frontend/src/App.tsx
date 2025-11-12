import { Route, Routes } from 'react-router-dom';
import './App.scss';
import { MainPage, PageOfBook } from './pages';
import { Layout } from './layout';

function App() {
    return (
        <Routes>
            <Route path='/' element={<Layout />}>
                <Route index element={<MainPage />} />
                <Route path='/page' element={<PageOfBook />} />
            </Route>
        </Routes>
    );
}

export default App;
