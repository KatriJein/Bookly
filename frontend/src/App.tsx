import { Route, Routes, useLocation } from 'react-router-dom';
import './App.scss';
import {
    AuthLayout,
    BooksPage,
    CollectionPage,
    FavoritesPage,
    LoginPage,
    MainPage,
    MyCollectionsPage,
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
                    <Route path='/books' element={<BooksPage />} />
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
                        path='/collection-page/:id'
                        element={<CollectionPage />}
                    />
                    <Route
                        path='/my-collections'
                        element={
                            <ProtectedRoute>
                                <MyCollectionsPage />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path='/favorites'
                        element={
                            <ProtectedRoute>
                                <FavoritesPage />
                            </ProtectedRoute>
                        }
                    />
                    {/* <Route path="/collection/:id" element={<CollectionPage />} /> */}
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
