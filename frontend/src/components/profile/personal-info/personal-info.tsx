import styles from './personal-info.module.scss';
import Person from '../../../assets/images/person.jpg';
import clsx from 'clsx';
import { EditButton } from '../../uikit';

interface PersonalInfoProps {
    user?: {
        name?: string;
        email?: string;
        phone?: string;
        avatar?: string;
    };
    editable?: boolean;
    onEdit?: () => void;
    buttonText?: string;
    buttonColor?: 'pink' | 'blue';
    onButtonClick?: () => void;
}

export function PersonalInfo({
    user = {
        name: 'Ladno Normis',
        email: 'ladno@normis',
        phone: '+7 999 999 99-99',
        avatar: Person,
    },
    editable = true,
    onEdit,
    buttonText = 'Изменить пароль',
    buttonColor = 'blue',
    onButtonClick,
}: PersonalInfoProps) {
    return (
        <div className={styles.personalInfo}>
            <img src={user.avatar} alt='Person' className={styles.avatar} />
            <div className={styles.info}>
                <div className={styles.name}>
                    <h2 className={styles.title}>{user.name}</h2>
                    <p className={styles.email}>e-mail: {user.email}</p>
                    <p className={styles.phone}>тел.: {user.phone}</p>
                </div>
                <button
                    className={clsx('button', buttonColor, styles.button)}
                    onClick={onButtonClick}
                >
                    {buttonText}
                </button>
            </div>
            {editable && (
                <EditButton onClick={onEdit} className={styles.edit} />
            )}
        </div>
    );
}
