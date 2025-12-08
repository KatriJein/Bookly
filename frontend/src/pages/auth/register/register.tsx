import clsx from 'clsx';
import { InputPassword, InputText } from '../../../components';
import styles from './register.module.scss';
import { useState } from 'react';
import {
    register,
    selectUserError,
    selectUserLoading,
    useDispatch,
    useSelector,
} from '../../../store';
import { useNavigate } from 'react-router-dom';

export const RegisterPage = () => {
    const [email, setEmail] = useState('');
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const dispatch = useDispatch();
    const navigate = useNavigate();

    const error = useSelector(selectUserError);
    const isLoading = useSelector(selectUserLoading);

    const isFormValid = () => {
        return (
            email.trim() !== '' &&
            login.trim() !== '' &&
            password.trim() !== '' &&
            confirmPassword.trim() !== '' &&
            password === confirmPassword
        );
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!isFormValid()) {
            return;
        }

        try {
            await dispatch(
                register({
                    login,
                    email,
                    password,
                })
            ).unwrap();
            navigate('/profile');
        } catch {
            // Ошибка уже будет в state
        }
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Регистрация</h1>
            <form onSubmit={handleSubmit} className={styles.container}>
                <InputText
                    title='E-mail'
                    value={email}
                    onChange={setEmail}
                    placeholder='Введите email'
                    isRequired
                />
                <InputText
                    title='Ваше имя'
                    value={login}
                    onChange={setLogin}
                    placeholder='Введите логин'
                    isRequired
                />
                <InputPassword
                    title='Пароль'
                    value={password}
                    onChange={setPassword}
                    placeholder='Введите пароль'
                    isRequired
                />
                <div className={styles.error}>
                    <InputPassword
                        title='Повторите пароль'
                        value={confirmPassword}
                        onChange={setConfirmPassword}
                        placeholder='Подтвердите пароль'
                        isRequired
                    />
                    {password &&
                        confirmPassword &&
                        password !== confirmPassword && (
                            <p className={styles.errorText}>
                                Пароли не совпадают
                            </p>
                        )}
                </div>

                {error && <p className={styles.errorText}>{error}</p>}

                <button
                    type='submit'
                    className={clsx('button', styles.button, styles.login)}
                    disabled={!isFormValid() || isLoading}
                >
                    {isLoading ? 'Регистрируемся...' : 'Зарегистрироваться'}
                </button>
            </form>

            <p className={styles.forgot}>
                У вас уже есть аккаунт?{' '}
                <a
                href="/login"
                    className={styles.link}
                    onClick={(e) => {
                        e.preventDefault();
                        navigate('/login');
                    }}
                >
                    Войдите
                </a>
            </p>
        </div>
    );
};
