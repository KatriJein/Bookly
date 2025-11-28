import styles from './personal-info.module.scss';
import { selectUser, useSelector } from '../../../store';
import { useEffect, useRef, useState, type ChangeEvent } from 'react';
import { InputText } from '../../auth';
import DefaultUser from '../../../assets/images/default-user.png';
import clsx from 'clsx';
import { EditButton } from '../../uikit';

interface PersonalInfoProps {
    editable?: boolean;
    onEdit?: () => void;
    buttonText?: string;
    buttonColor?: 'pink' | 'blue';
    onButtonClick?: () => void;
    isAuthor?: boolean;
    authorText?: string;
}

export function PersonalInfo({
    editable = false,
    buttonText = 'Изменить пароль',
    buttonColor = 'blue',
    onButtonClick,
    isAuthor = false,
    authorText = 'Автор',
}: PersonalInfoProps) {
    const currentUser = useSelector(selectUser);

    const [isEditing, setIsEditing] = useState(false);
    const [name, setName] = useState(currentUser?.login || '');
    const [email, setEmail] = useState(currentUser?.email || '');
    const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        if (!isEditing) {
            setName(currentUser?.login || '');
            setEmail(currentUser?.email || '');
            setAvatarPreview(null);
        }
    }, [isEditing, currentUser]);

    if (!currentUser) {
        return null;
    }
    const isEditableMode = editable && !isAuthor;

    const avatarUrl = avatarPreview || currentUser.avatarUrl || DefaultUser;

    const handleEditClick = () => {
        if (isEditableMode) {
            setIsEditing(true);
        }
    };

    const handleSave = () => {
        console.log('Сохраняем профиль:', {
            name,
            email,
            avatar: avatarPreview,
        });
        setIsEditing(false);
        // TODO: вызвать API для обновления профиля
    };

    const handleCancel = () => {
        setIsEditing(false);
    };

    const handleAvatarClick = () => {
        if (isEditing && fileInputRef.current) {
            fileInputRef.current.click();
        }
    };

    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            const url = URL.createObjectURL(file);
            setAvatarPreview(url);
        }
    };

    return (
        <div
            className={clsx(styles.personalInfo, {
                [styles.viewMode]: !isEditing,
            })}
        >
            <div
                className={clsx(
                    styles.avatarWrapper,
                    isEditing && isEditableMode && styles.editableAvatar
                )}
                onClick={handleAvatarClick}
            >
                <img src={avatarUrl} alt='Аватар' className={styles.avatar} />
                {isEditing && isEditableMode && (
                    <div className={styles.editIconOverlay}>
                        <span className='material-icons'>edit</span>
                    </div>
                )}
                <input
                    type='file'
                    ref={fileInputRef}
                    onChange={handleFileChange}
                    accept='image/*'
                    style={{ display: 'none' }}
                />
            </div>

            <div className={styles.info}>
                <div className={styles.name}>
                    {isEditing && isEditableMode ? (
                        <InputText
                            title='Логин'
                            value={name}
                            onChange={setName}
                            placeholder='Введите логин'
                        />
                    ) : (
                        <h2 className={styles.title}>
                            {isAuthor ? authorText : currentUser.login}
                        </h2>
                    )}

                    {isAuthor ? (
                        <p className={styles.email}>Автор подборки</p>
                    ) : isEditing && isEditableMode ? (
                        <InputText
                            title='E-mail'
                            value={email}
                            onChange={setEmail}
                            placeholder='Введите e-mail'
                        />
                    ) : (
                        <p className={styles.email}>
                            e-mail: {currentUser.email}
                        </p>
                    )}
                </div>

                {!isEditing && (
                    <button
                        className={clsx('button', buttonColor, styles.button)}
                        onClick={onButtonClick}
                    >
                        {buttonText}
                    </button>
                )}

                {isEditing && isEditableMode && (
                    <div className={styles.editActions}>
                        <button
                            className={styles.cancelButton}
                            onClick={handleCancel}
                        >
                            Отмена
                        </button>
                        <button
                            className={styles.saveButton}
                            onClick={handleSave}
                        >
                            Сохранить
                        </button>
                    </div>
                )}
            </div>

            {!isEditing && editable && isEditableMode && (
                <EditButton onClick={handleEditClick} className={styles.edit} />
            )}
        </div>
    );
}
