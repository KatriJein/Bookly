import type { ChangeEvent, InputHTMLAttributes } from 'react';
import styles from './input-text.module.scss';

interface InputTextProps
    extends Omit<InputHTMLAttributes<HTMLInputElement>, 'value' | 'onChange'> {
    title: string;
    value: string;
    onChange: (value: string) => void;
    isRequired?: boolean;
}

export const InputText = ({
    title,
    value,
    onChange,
    isRequired = false,
    ...inputProps
}: InputTextProps) => {
    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        onChange(e.target.value);
    };

    return (
        <div className={styles.container}>
            <label className={styles.title}>{title} {isRequired && <span className={styles.asterisk}>*</span>}</label>
            <input
                className={styles.input}
                type='text'
                value={value}
                onChange={handleChange}
                {...inputProps}
            />
        </div>
    );
};
