import { Route, Routes, useLocation } from 'react-router-dom';
import './App.scss';
import {
    AuthLayout,
    CollectionPage,
    LoginPage,
    MainPage,
    PageOfBook,
    RegisterPage,
} from './pages';
import { Layout, LayoutMain } from './layout';
import { AuthorProfile, OtherProfile, PersonalProfile } from './pages/profiles';
import { ToastContainer } from 'react-toastify';
import { ProtectedRoute } from './utils';

function App() {
    const location = useLocation();
    // const dispatch = useDispatch();

    return (
        <>
            <Routes location={location}>
                <Route path='/' element={<LayoutMain />}>
                    <Route index element={<MainPage />} />
                </Route>

                <Route path='/' element={<Layout />}>
                    <Route path='/book/:id' element={<PageOfBook />} />
                    <Route
                        path='/profile'
                        element={
                            <ProtectedRoute>
                                <PersonalProfile />
                            </ProtectedRoute>
                        }
                    />
                    <Route path='/other-profile' element={<OtherProfile />} />
                    <Route path='/author-profile' element={<AuthorProfile />} />
                    <Route
                        path='/collection-page'
                        element={<CollectionPage />}
                    />
                </Route>

                <Route
                    path='/login'
                    element={
                        <ProtectedRoute onlyUnAuth>
                            <AuthLayout>
                                <LoginPage />
                            </AuthLayout>
                        </ProtectedRoute>
                    }
                />
                <Route
                    path='/register'
                    element={
                        <ProtectedRoute onlyUnAuth>
                            <AuthLayout>
                                <RegisterPage />
                            </AuthLayout>
                        </ProtectedRoute>
                    }
                />
            </Routes>
            <ToastContainer
                position='top-right'
                autoClose={2000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
            />
        </>
    );
}

export default App;
