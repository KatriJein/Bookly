import clsx from 'clsx';
import styles from './drop-down-single.module.scss';
import { useState, type ReactNode } from 'react';

interface DropDownSingleProps {
    items: {
        id: string;
        content: ReactNode;
        disabled?: boolean;
    }[];
    onApply?: (selectedId: string | null) => void;
    initialSelectedId?: string | null;
    applyText?: string;
    className?: string;
    name?: string;
}

export function DropDownSingle({
    items,
    onApply,
    initialSelectedId = null,
    applyText = 'Применить',
    className = '',
    name = 'dropdown-radio',
}: DropDownSingleProps) {
    const [selectedId, setSelectedId] = useState<string | null>(
        initialSelectedId
    );

    const handleItemChange = (id: string) => {
        setSelectedId(id);
    };

    const handleApply = () => {
        onApply?.(selectedId);
    };

    return (
        <div className={clsx(styles.dropDownSingle, className)}>
            <div
                className={styles.list}
                role='radiogroup'
                aria-label='Выбор опции'
            >
                {items.map((item) => (
                    <label
                        key={item.id}
                        className={clsx(
                            styles.item,
                            item.disabled && styles.disabled
                        )}
                    >
                        <input
                            type='radio'
                            name={name}
                            value={item.id}
                            checked={selectedId === item.id}
                            onChange={() => handleItemChange(item.id)}
                            disabled={item.disabled}
                            className={styles.radioInput}
                        />
                        <span className={styles.customRadio} />
                        <div className={styles.content}>{item.content}</div>
                    </label>
                ))}
            </div>
            <button
                className={clsx(styles.applyButton, 'button')}
                onClick={handleApply}
            >
                {applyText}
            </button>
        </div>
    );
}
