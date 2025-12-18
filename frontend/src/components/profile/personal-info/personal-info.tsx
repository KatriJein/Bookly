// import styles from './personal-info.module.scss';
// import {
//     selectUser,
//     updateAvatar,
//     updateUser,
//     useDispatch,
//     useSelector,
// } from '../../../store';
// import { useEffect, useRef, useState, type ChangeEvent } from 'react';
// import { InputText } from '../../auth';
// import DefaultUser from '../../../assets/images/default-user.png';
// import clsx from 'clsx';
// import { EditButton } from '../../uikit';
// import { toast } from 'react-toastify';
// import { Modal } from '../../modal';
// import { ChangePassword } from '../change-password';

// interface PersonalInfoProps {
//     editable?: boolean;
//     onEdit?: () => void;
//     buttonText?: string;
//     buttonColor?: 'pink' | 'blue';
//     onButtonClick?: () => void;
//     isAuthor?: boolean;
//     authorText?: string;
// }

// export function PersonalInfo({
//     editable = false,
//     buttonText = 'Изменить пароль',
//     buttonColor = 'blue',
//     // onButtonClick,
//     isAuthor = false,
//     authorText = 'Автор',
// }: PersonalInfoProps) {
//     const currentUser = useSelector(selectUser);
//     const dispatch = useDispatch();

//     const [isEditing, setIsEditing] = useState(false);
//     const [name, setName] = useState(currentUser?.login || '');
//     const [email, setEmail] = useState(currentUser?.email || '');
//     const [avatarFile, setAvatarFile] = useState<File | null>(null);
//     const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
//     const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
//     const fileInputRef = useRef<HTMLInputElement>(null);

//     useEffect(() => {
//         if (!isEditing) {
//             setName(currentUser?.login || '');
//             setEmail(currentUser?.email || '');
//             setAvatarFile(null);
//             setAvatarPreview(null);
//         }
//     }, [isEditing, currentUser]);

//     if (!currentUser) {
//         return null;
//     }

//     const isEditableMode = editable && !isAuthor;
//     const avatarUrl = avatarPreview || currentUser.avatarUrl || DefaultUser;

//     const handleEditClick = () => {
//         if (isEditableMode) {
//             setIsEditing(true);
//         }
//     };

//     const handleSave = async () => {
//         try {
//             const profileChanges: { login?: string; email?: string } = {};
//             if (name !== currentUser.login) profileChanges.login = name;
//             if (email !== currentUser.email) profileChanges.email = email;

//             if (Object.keys(profileChanges).length > 0) {
//                 await dispatch(
//                     updateUser({ userId: currentUser.id, data: profileChanges })
//                 ).unwrap();
//             }

//             if (avatarFile) {
//                 await dispatch(
//                     updateAvatar({ userId: currentUser.id, file: avatarFile })
//                 ).unwrap();
//             }

//             toast.success('Профиль успешно обновлён');
//             setIsEditing(false);
//         } catch (error) {
//             const message =
//                 error instanceof Error
//                     ? error.message
//                     : 'Не удалось сохранить изменения';
//             toast.error(message);
//         }
//     };

//     const handleCancel = () => {
//         setIsEditing(false);
//     };

//     const handleAvatarClick = () => {
//         if (isEditing && fileInputRef.current) {
//             fileInputRef.current.click();
//         }
//     };

//     const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
//         const file = e.target.files?.[0];
//         if (file) {
//             setAvatarFile(file);
//             const url = URL.createObjectURL(file);
//             setAvatarPreview(url);
//         }
//     };

//     return (
//         <div
//             className={clsx(styles.personalInfo, {
//                 [styles.viewMode]: !isEditing,
//             })}
//         >
//             <div
//                 className={clsx(
//                     styles.avatarWrapper,
//                     isEditing && isEditableMode && styles.editableAvatar
//                 )}
//                 onClick={handleAvatarClick}
//             >
//                 <img src={avatarUrl} alt='Аватар' className={styles.avatar} />
//                 {isEditing && isEditableMode && (
//                     <div className={styles.editIconOverlay}>
//                         <span className='material-icons'>edit</span>
//                     </div>
//                 )}
//                 <input
//                     type='file'
//                     ref={fileInputRef}
//                     onChange={handleFileChange}
//                     accept='image/*'
//                     style={{ display: 'none' }}
//                 />
//             </div>

//             <div className={styles.info}>
//                 <div className={styles.name}>
//                     {isEditing && isEditableMode ? (
//                         <InputText
//                             title='Логин'
//                             value={name}
//                             onChange={setName}
//                             placeholder='Введите логин'
//                         />
//                     ) : (
//                         <h2 className={styles.title}>
//                             {isAuthor ? authorText : currentUser.login}
//                         </h2>
//                     )}

//                     {isAuthor ? (
//                         <p className={styles.email}>Автор подборки</p>
//                     ) : isEditing && isEditableMode ? (
//                         <InputText
//                             title='E-mail'
//                             value={email}
//                             onChange={setEmail}
//                             placeholder='Введите e-mail'
//                         />
//                     ) : (
//                         <p className={styles.email}>
//                             e-mail: {currentUser.email}
//                         </p>
//                     )}
//                 </div>

//                 {!isEditing && (
//                     <button
//                         className={clsx('button', buttonColor, styles.button)}
//                         onClick={() => setIsPasswordModalOpen(true)}
//                     >
//                         {buttonText}
//                     </button>
//                 )}

//                 {isEditing && isEditableMode && (
//                     <div className={styles.editActions}>
//                         <button
//                             className={styles.cancelButton}
//                             onClick={handleCancel}
//                         >
//                             Отмена
//                         </button>
//                         <button
//                             className={styles.saveButton}
//                             onClick={handleSave}
//                         >
//                             Сохранить
//                         </button>
//                     </div>
//                 )}
//             </div>

//             {!isEditing && editable && isEditableMode && (
//                 <EditButton onClick={handleEditClick} className={styles.edit} />
//             )}
//             {isPasswordModalOpen && (
//                 <Modal
//                     isOpen={isPasswordModalOpen}
//                     onClose={() => setIsPasswordModalOpen(false)}
//                     width={30}
//                 >
//                     <ChangePassword
//                         onSuccess={() => setIsPasswordModalOpen(false)}
//                     />
//                 </Modal>
//             )}
//         </div>
//     );
// }
// personal-info.tsx

import styles from './personal-info.module.scss';
import {
    selectUser,
    updateAvatar,
    updateUser,
    useDispatch,
    useSelector,
} from '../../../store';
import { useEffect, useRef, useState, type ChangeEvent } from 'react';
import { InputText } from '../../auth';
import DefaultUser from '../../../assets/images/default-user.png';
import clsx from 'clsx';
import { EditButton } from '../../uikit';
import { toast } from 'react-toastify';
import { Modal } from '../../modal';
import { ChangePassword } from '../change-password';

interface PersonalInfoProps {
    // Режим "чужой профиль"
    isExternal?: boolean;
    externalName?: string; // Имя для внешнего профиля
    externalEmail?: string; // Email (опционально)
    externalAvatarUrl?: string | null; // Аватарка
    buttonText?: string; // Текст кнопки
    buttonColor?: 'pink' | 'blue';
    onButtonClick?: () => void; // Действие при нажатии кнопки

    // Режим "личный кабинет" (по умолчанию)
    editable?: boolean;
    isAuthor?: boolean;
    authorText?: string;
}

export function PersonalInfo({
    isExternal = false,
    externalName,
    externalEmail,
    externalAvatarUrl,
    buttonText = 'Изменить пароль',
    buttonColor = 'blue',
    onButtonClick,

    // Для личного кабинета
    editable = false,
    isAuthor = false,
    authorText = 'Автор',
}: PersonalInfoProps) {
    const currentUser = useSelector(selectUser);
    const dispatch = useDispatch();

    // --- Состояния для редактирования (только в личном кабинете) ---
    const [isEditing, setIsEditing] = useState(false);
    const [name, setName] = useState(currentUser?.login || '');
    const [email, setEmail] = useState(currentUser?.email || '');
    const [avatarFile, setAvatarFile] = useState<File | null>(null);
    const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
    const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Сброс состояния при выходе из режима редактирования
    useEffect(() => {
        if (!isEditing) {
            setName(currentUser?.login || '');
            setEmail(currentUser?.email || '');
            setAvatarFile(null);
            setAvatarPreview(null);
        }
    }, [isEditing, currentUser]);

    // --- Определяем, какие данные показывать ---
    const isPersonalMode = !isExternal;
    const isEditableMode = isPersonalMode && editable && !isAuthor;

    if (isExternal) {
        // --- Внешний режим: только отображение ---
        const avatarUrl = externalAvatarUrl
            ? externalAvatarUrl.startsWith('person')
                ? `https://bookly-files-bucket.s3.yandexcloud.net/${externalAvatarUrl}`
                : externalAvatarUrl
            : DefaultUser;
        const displayName = externalName || 'Пользователь';

        return (
            <div className={styles.personalInfo}>
                <div className={styles.avatarWrapper}>
                    <img
                        src={avatarUrl}
                        alt='Аватар'
                        className={styles.avatar}
                    />
                </div>
                <div className={styles.info}>
                    <div className={styles.name}>
                        <h2 className={styles.title}>{displayName}</h2>
                        {externalEmail && (
                            <p className={styles.email}>
                                e-mail: {externalEmail}
                            </p>
                        )}
                    </div>
                    {onButtonClick && (
                        <button
                            className={clsx(
                                'button',
                                buttonColor,
                                styles.button
                            )}
                            onClick={onButtonClick}
                        >
                            {buttonText}
                        </button>
                    )}
                </div>
            </div>
        );
    }

    // --- Личный кабинет (как раньше) ---
    if (!currentUser) {
        return null;
    }

    const avatarUrl = avatarPreview || currentUser.avatarUrl || DefaultUser;
    const displayTitle = isAuthor ? authorText : currentUser.login;

    const handleEditClick = () => {
        if (isEditableMode) {
            setIsEditing(true);
        }
    };

    const handleSave = async () => {
        try {
            const profileChanges: { login?: string; email?: string } = {};
            if (name !== currentUser.login) profileChanges.login = name;
            if (email !== currentUser.email) profileChanges.email = email;

            if (Object.keys(profileChanges).length > 0) {
                await dispatch(
                    updateUser({ userId: currentUser.id, data: profileChanges })
                ).unwrap();
            }

            if (avatarFile) {
                await dispatch(
                    updateAvatar({ userId: currentUser.id, file: avatarFile })
                ).unwrap();
            }

            toast.success('Профиль успешно обновлён');
            setIsEditing(false);
        } catch (error) {
            const message =
                error instanceof Error
                    ? error.message
                    : 'Не удалось сохранить изменения';
            toast.error(message);
        }
    };

    const handleCancel = () => {
        setIsEditing(false);
    };

    const handleAvatarClick = () => {
        if (isEditing && isEditableMode && fileInputRef.current) {
            fileInputRef.current.click();
        }
    };

    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setAvatarFile(file);
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
                        <h2 className={styles.title}>{displayTitle}</h2>
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
                        onClick={() => setIsPasswordModalOpen(true)}
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
            {isPasswordModalOpen && (
                <Modal
                    isOpen={isPasswordModalOpen}
                    onClose={() => setIsPasswordModalOpen(false)}
                    width={30}
                >
                    <ChangePassword
                        onSuccess={() => setIsPasswordModalOpen(false)}
                    />
                </Modal>
            )}
        </div>
    );
}
