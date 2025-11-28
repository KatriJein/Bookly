import styles from './auth-layout.module.scss';
import Arrow from '../../../assets/svg/auth-arrow-left.svg';
import { useNavigate } from 'react-router-dom';

interface AuthLayoutProps {
    children: React.ReactNode;
}

export const AuthLayout = ({ children }: AuthLayoutProps) => {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate('/');
    };

    return (
        <div className={styles.container}>
            <div className={styles.content}>
                <button className={styles.button} onClick={handleClick}>
                    <img src={Arrow} alt='Arrow' className={styles.arrow} />
                    На главную
                </button>
                <div className={styles.children}>{children}</div>
            </div>
        </div>
    );
};
