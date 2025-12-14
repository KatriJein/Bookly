import { createBookCollection, selectUser, useDispatch, useSelector } from '../../../store';
import { InputText } from '../../auth';
import styles from './create-collection.module.scss';
import { useState } from 'react';
import { toast } from 'react-toastify'; 

interface CreateCollectionProps {
    onSuccess?: () => void; 
}

export const CreateCollection = ({ onSuccess }: CreateCollectionProps = {}) => {
    const dispatch = useDispatch();
    const user = useSelector(selectUser);

    const [title, setTitle] = useState('');
    const [isPublic, setIsPublic] = useState(false);

    const handleCreate = async () => {
        if (!user || !title.trim()) return;

        try {
            await dispatch(
                createBookCollection({
                    title: title.trim(),
                    isPublic,
                    userId: user.id,
                })
            ).unwrap();

            toast.success('Подборка успешно создана!');
            setTitle(''); 
            onSuccess?.();
        } catch (error: unknown) {
            const message =
                error instanceof Error ? error.message : 'Не удалось создать подборку';
            toast.error(message);
        }
    };

    const isTitleValid = title.trim() !== '';

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>Создание подборки</h1>
            <InputText
                title='Название подборки'
                value={title}
                onChange={setTitle}
                placeholder='Введите название'
            />
            <div className={styles.checkboxWrapper}>
                <label className={styles.item}>
                    <input
                        type='checkbox'
                        className={styles.checkboxInput}
                        checked={isPublic}
                        onChange={(e) => setIsPublic(e.target.checked)}
                    />
                    <span className={styles.customCheckbox}></span>
                    <span className={styles.itemText}>Сделать публичной</span>
                </label>
            </div>
            <button
                className={styles.createButton}
                onClick={handleCreate}
                disabled={!isTitleValid}
            >
                Создать
            </button>
        </div>
    );
};