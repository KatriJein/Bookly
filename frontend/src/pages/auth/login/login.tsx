import clsx from 'clsx';
import { InputPassword, InputText } from '../../../components';
import styles from './login.module.scss';
import { useState } from 'react';
import {
    login,
    selectUserError,
    selectUserLoading,
    useDispatch,
    useSelector,
} from '../../../store';
import { useNavigate } from 'react-router-dom';

export const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const dispatch = useDispatch();
    const navigate = useNavigate();

    const error = useSelector(selectUserError);
    const isLoading = useSelector(selectUserLoading);

    const isFormValid = () => {
        return email.trim() !== '' && password.trim() !== '';
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!isFormValid()) {
            return;
        }

        try {
            await dispatch(login({ login: email, password })).unwrap();
            navigate('/profile');
        } catch {
            // Ошибка уже в Redux → отобразится ниже
        }
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Вход в аккаунт</h1>
            <form onSubmit={handleSubmit} className={styles.container}>
                <InputText
                    title='Email'
                    value={email}
                    onChange={setEmail}
                    placeholder='Введите email'
                />
                <InputPassword
                    title='Пароль'
                    value={password}
                    onChange={setPassword}
                    placeholder='Введите пароль'
                />

                {error && <p className={styles.errorText}>{error}</p>}

                <button
                    type='submit'
                    className={clsx('button', styles.button, styles.login)}
                    disabled={!isFormValid() || isLoading}
                >
                    {isLoading ? 'Входим...' : 'Войти'}
                </button>
            </form>

            <div className={styles.divider}>
                <span className={styles.text}>ИЛИ</span>
            </div>

            <button
                className={clsx('button', styles.button, styles.register)}
                onClick={() => navigate('/register')}
            >
                Зарегистрироваться
            </button>

            <p className={styles.forgot}>
                Забыли пароль? Восстановите его{' '}
                <a href='#' className={styles.link}>
                    здесь
                </a>
            </p>
        </div>
    );
};
