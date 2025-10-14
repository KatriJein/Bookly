import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.scss';
import App from './App.tsx';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import store from './store/store.ts';
import { HelmetProvider } from 'react-helmet-async';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <HelmetProvider>
            <Provider store={store}>
                <BrowserRouter>
                    <App />
                </BrowserRouter>
            </Provider>
        </HelmetProvider>
    </StrictMode>
);
