import styles from './edit-button.module.scss';
import Pencil from '../../../assets/svg/pencil.svg';
import clsx from 'clsx';

interface EditButtonProps {
    onClick: (() => void) | undefined;
    className?: string;
}

export function EditButton(props: EditButtonProps) {
    const { onClick, className } = props;
    return (
        <button
            onClick={onClick}
            className={clsx(styles.editButton, className)}
        >
            <img src={Pencil} alt='edit' className={styles.icon} />
        </button>
    );
}
