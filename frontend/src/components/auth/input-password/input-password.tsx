import { useState } from 'react';
import styles from './input-password.module.scss';

interface InputPasswordProps {
    title: string;
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
    name?: string;
    disabled?: boolean;
    isRequired?: boolean;
}

export const InputPassword = ({
    title,
    value,
    onChange,
    placeholder,
    name,
    disabled,
    isRequired = false,
}: InputPasswordProps) => {
    const [isPasswordVisible, setIsPasswordVisible] = useState(false);

    const toggleVisibility = () => {
        setIsPasswordVisible((prev) => !prev);
    };

    const inputType = isPasswordVisible ? 'text' : 'password';

    return (
        <div className={styles.container}>
            <label className={styles.title}>
                {title}
                {isRequired && <span className={styles.asterisk}>*</span>}
            </label>
            <div className={styles.inputWrapper}>
                <input
                    className={styles.input}
                    type={inputType}
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                    placeholder={placeholder}
                    name={name}
                    disabled={disabled}
                />
                <button
                    type='button'
                    className={styles.toggleButton}
                    onClick={toggleVisibility}
                    aria-label={
                        isPasswordVisible ? 'Скрыть пароль' : 'Показать пароль'
                    }
                >
                    <span className='material-icons'>
                        {isPasswordVisible ? 'visibility_off' : 'visibility'}
                    </span>
                </button>
            </div>
        </div>
    );
};
