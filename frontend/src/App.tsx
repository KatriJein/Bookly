import { Route, Routes } from 'react-router-dom';
import './App.scss';
import { AuthLayout, CollectionPage, LoginPage, MainPage, PageOfBook, RegisterPage } from './pages';
import { Layout, LayoutMain } from './layout';
import { AuthorProfile, OtherProfile, PersonalProfile } from './pages/profiles';

function App() {
    return (
        <Routes>
            <Route path='/' element={<LayoutMain />}>
                <Route index element={<MainPage />} />
            </Route>

            <Route path='/' element={<Layout />}>
                <Route path='/book/:id' element={<PageOfBook />} />
                <Route path='/profile' element={<PersonalProfile />} />
                <Route path='/other-profile' element={<OtherProfile />} />
                <Route path='/author-profile' element={<AuthorProfile />} />
                <Route path='/collection-page' element={<CollectionPage />} />
            </Route>

            <Route
                path='/login'
                element={
                    <AuthLayout>
                        <LoginPage />
                    </AuthLayout>
                }
            />
             <Route
                path='/register'
                element={
                    <AuthLayout>
                        <RegisterPage />
                    </AuthLayout>
                }
            />
        </Routes>
    );
}

export default App;
