import clsx from 'clsx';
import { InputPassword } from '../../auth';
import styles from './change-password.module.scss';
import {
    selectUser,
    updatePassword,
    useDispatch,
    useSelector,
} from '../../../store';
import { useState } from 'react';
import { toast } from 'react-toastify';

interface ChangePasswordProps {
    onSuccess?: () => void;
}

export const ChangePassword = ({ onSuccess }: ChangePasswordProps = {}) => {
    const dispatch = useDispatch();
    const user = useSelector(selectUser);

    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSave = async () => {
        setError(null);

        if (!oldPassword.trim()) {
            setError('Введите старый пароль');
            return;
        }
        if (!newPassword.trim()) {
            setError('Введите новый пароль');
            return;
        }
        if (newPassword !== confirmPassword) {
            setError('Пароли не совпадают');
            return;
        }

        if (!user) {
            setError('Пользователь не авторизован');
            return;
        }

        setIsLoading(true);

        try {
            await dispatch(
                updatePassword({
                    userId: user.id,
                    data: {
                        oldPassword,
                        newPassword,
                    },
                })
            ).unwrap();

            toast.success('Пароль успешно изменён');
            setOldPassword('');
            setNewPassword('');
            setConfirmPassword('');
             onSuccess?.(); 
        } catch (err: unknown) {
            const message =
                err instanceof Error
                    ? err.message
                    : 'Не удалось изменить пароль';
            setError(message);
            toast.error(message);
        } finally {
            setIsLoading(false);
        }
    };

    const isFormValid =
        oldPassword.trim() !== '' &&
        newPassword.trim() !== '' &&
        confirmPassword.trim() !== '' &&
        newPassword === confirmPassword;

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Установить новый пароль</h1>
            <InputPassword
                title='Старый пароль'
                value={oldPassword}
                onChange={setOldPassword}
                placeholder='Введите старый пароль'
            />
            <InputPassword
                title='Новый пароль'
                value={newPassword}
                onChange={setNewPassword}
                placeholder='Введите новый пароль'
            />
            <div>
                <InputPassword
                    title='Повторите пароль'
                    value={confirmPassword}
                    onChange={setConfirmPassword}
                    placeholder='Повторите новый пароль'
                />
                {confirmPassword.trim() !== '' && newPassword !== confirmPassword && (
                    <p className={styles.error}>Пароли не совпадают</p>
                )}
            </div>

            {error && <p className={styles.error}>{error}</p>}

            <button
                className={clsx('button', 'pink', styles.button)}
                onClick={handleSave}
                disabled={isLoading || !isFormValid} 
            >
                {isLoading ? 'Сохранение...' : 'Сохранить'}
            </button>
        </div>
    );
};