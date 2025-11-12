import { useState } from 'react';
import styles from './search-bar.module.scss';
import clsx from 'clsx';

interface SearchBarProps {
    placeholder?: string;
    onSearch?: (query: string) => void;
    className?: string;
}

export function SearchBar({
    placeholder = 'Поиск',
    onSearch,
    className = '',
}: SearchBarProps) {
    const [searchQuery, setSearchQuery] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSearch?.(searchQuery);
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchQuery(e.target.value);
    };

    const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Enter') {
            handleSubmit(e);
        }
    };

    return (
        <form
            className={clsx(styles.searchBar, className)}
            onSubmit={handleSubmit}
        >
            <div className={styles.searchContainer}>
                <input
                    type='text'
                    value={searchQuery}
                    onChange={handleInputChange}
                    onKeyDown={handleKeyPress}
                    placeholder={placeholder}
                    className={styles.input}
                />
                <button
                    type='submit'
                    className={clsx(styles.searchButton, 'button', 'blue')}
                    disabled={!searchQuery.trim()}
                >
                    Найти
                </button>
            </div>
        </form>
    );
}
