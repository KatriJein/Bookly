import styles from './drop-down-button.module.scss';
import { useState, useRef, useEffect, type ReactNode } from 'react';
import clsx from 'clsx';
import Icon from '../../../assets/svg/sort_up.svg';
import { DropDownMultiple } from '../drop-down-list/multiple';
import { DropDownSingle } from '../drop-down-list/single';

interface DropDownButtonProps {
    text: string;
    color: 'blue' | 'pink';
    listType: 'single' | 'multiple';
    items: string[] | { id: string; content: ReactNode; disabled?: boolean }[];
    onApply?: (selected: unknown) => void;
    initialSelection?: unknown;
    applyText?: string;
    searchPlaceholder?: string;
    className?: string;
}

export function DropDownButton({
    text,
    color,
    listType,
    items,
    onApply,
    initialSelection,
    applyText = 'Применить',
    searchPlaceholder = 'Поиск',
    className = '',
}: DropDownButtonProps) {
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const buttonRef = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (
                dropdownRef.current &&
                !dropdownRef.current.contains(event.target as Node) &&
                buttonRef.current &&
                !buttonRef.current.contains(event.target as Node)
            ) {
                setIsOpen(false);
            }
        }

        if (isOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isOpen]);

    const toggleDropdown = () => {
        setIsOpen(!isOpen);
    };

    const handleApply = (selected: unknown) => {
        onApply?.(selected);
        setIsOpen(false);
    };

    const isBlue = color === 'blue';
    const isMultiple = listType === 'multiple';

    return (
        <div className={clsx(styles.dropDownButtonContainer, className)}>
            <button
                ref={buttonRef}
                className={clsx(
                    styles.dropDownButton,
                    isBlue ? styles.blue : styles.pink,
                    isOpen && styles.open
                )}
                onClick={toggleDropdown}
                type='button'
                aria-expanded={isOpen}
                aria-haspopup='listbox'
            >
                {isBlue && <img src={Icon} alt='' className={styles.icon} />}
                <span className={styles.buttonText}>{text}</span>
                <ArrowDown
                    className={clsx(
                        styles.arrow,
                        isOpen && styles.arrowRotated
                    )}
                    color={isBlue ? 'white' : 'pink'}
                />
            </button>

            {isOpen && (
                <div className={styles.dropdownContent} ref={dropdownRef}>
                    {isMultiple ? (
                        <DropDownMultiple
                            items={items as string[]}
                            onApply={handleApply}
                            initialSelectedItems={initialSelection as string[]}
                            applyText={applyText}
                            searchPlaceholder={searchPlaceholder}
                        />
                    ) : (
                        <DropDownSingle
                            items={
                                items as {
                                    id: string;
                                    content: ReactNode;
                                    disabled?: boolean;
                                }[]
                            }
                            onApply={handleApply}
                            initialSelectedId={
                                initialSelection as string | null
                            }
                            applyText={applyText}
                        />
                    )}
                </div>
            )}
        </div>
    );
}

interface ArrowDownProps {
    className?: string;
    size?: number;
    color?: 'white' | 'pink';
}

export function ArrowDown({
    className = '',
    size = 10,
    color = 'white',
}: ArrowDownProps) {
    return (
        <svg
            className={clsx(className, styles[`arrow-${color}`])}
            width={size}
            height='8'
            viewBox='0 0 12 8'
            fill='none'
            xmlns='http://www.w3.org/2000/svg'
        >
            <path
                d='M1 1L6 6L11 1'
                stroke='currentColor'
                strokeWidth='2'
                strokeLinecap='round'
            />
        </svg>
    );
}
