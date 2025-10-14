import { Route, Routes } from 'react-router-dom';
import './App.scss';
import { MainPage } from './main/page/main-page';

function App() {
    return (
        <>
            <Routes>
                <Route path='/' element={<MainPage />} />
            </Routes>
        </>
    );
}

export default App;
