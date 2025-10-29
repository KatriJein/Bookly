import styles from './drop-down-multiple.module.scss';
import { useState, useMemo } from 'react';
import Search from '../../../../assets/svg/search.svg';
import clsx from 'clsx';

interface DropDownMultipleProps {
    items: string[];
    onApply?: (selectedItems: string[]) => void;
    initialSelectedItems?: string[];
    applyText?: string;
    searchPlaceholder?: string;
    className?: string;
}

export function DropDownMultiple({
    items,
    onApply,
    initialSelectedItems = [],
    applyText = 'Применить',
    searchPlaceholder = 'Поиск',
    className = '',
}: DropDownMultipleProps) {
    const [selectedItems, setSelectedItems] =
        useState<string[]>(initialSelectedItems);
    const [searchQuery, setSearchQuery] = useState('');

    const filteredItems = useMemo(() => {
        if (!searchQuery.trim()) return items;
        return items.filter((item) =>
            item.toLowerCase().includes(searchQuery.toLowerCase())
        );
    }, [items, searchQuery]);

    const handleItemToggle = (item: string) => {
        setSelectedItems((prev) =>
            prev.includes(item)
                ? prev.filter((i) => i !== item)
                : [...prev, item]
        );
    };

    const handleApply = () => {
        onApply?.(selectedItems);
    };

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchQuery(e.target.value);
    };

    const handleSearchSubmit = (e: React.FormEvent) => {
        e.preventDefault();
    };

    return (
        <div className={clsx(styles.dropDownMultiple, className)}>
            <form className={styles.searchForm} onSubmit={handleSearchSubmit}>
                <div className={styles.searchContainer}>
                    <input
                        type='text'
                        value={searchQuery}
                        onChange={handleSearchChange}
                        placeholder={searchPlaceholder}
                        className={styles.searchInput}
                    />
                    <button
                        type='submit'
                        className={clsx(styles.searchButton, 'button')}
                    >
                        <img
                            src={Search}
                            alt='Search'
                            className={styles.searchIcon}
                        />
                    </button>
                </div>
            </form>

            <div className={styles.listContainer}>
                {filteredItems.length > 0 ? (
                    <div
                        className={styles.list}
                        role='listbox'
                        aria-multiselectable='true'
                    >
                        {filteredItems.map((item, index) => (
                            <label
                                key={index}
                                className={clsx(
                                    styles.item,
                                    selectedItems.includes(item) &&
                                        styles.selected
                                )}
                            >
                                <input
                                    type='checkbox'
                                    checked={selectedItems.includes(item)}
                                    onChange={() => handleItemToggle(item)}
                                    className={styles.checkboxInput}
                                />
                                <span className={styles.customCheckbox} />
                                <span className={styles.itemText}>{item}</span>
                            </label>
                        ))}
                    </div>
                ) : (
                    <div className={styles.emptyState}>
                        {searchQuery ? 'Ничего не найдено' : 'Список пуст'}
                    </div>
                )}
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
