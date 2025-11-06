import { Route, Routes } from 'react-router-dom';
import './App.scss';
import { PageOfBook } from './pages';
import { Layout } from './layout';

function App() {
    return (
        <Routes>
            <Route path='/' element={<Layout />}>
                <Route index element={<PageOfBook />} />
            </Route>
        </Routes>
    );
}

export default App;
