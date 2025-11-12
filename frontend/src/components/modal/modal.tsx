import { useEffect, useRef } from 'react';
import styles from './modal.module.scss';

interface ModalProps {
    children: React.ReactNode;
    isOpen: boolean;
    onClose: () => void;
    width?: number;
    height?: number;
}

export function Modal({
    children,
    isOpen,
    onClose,
    width = 60,
    height,
}: ModalProps) {
    const modalRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleEscape = (event: KeyboardEvent) => {
            if (event.key === 'Escape') {
                onClose();
            }
        };

        if (isOpen) {
            document.addEventListener('keydown', handleEscape);
            document.body.style.overflow = 'hidden';
        }

        return () => {
            document.removeEventListener('keydown', handleEscape);
            document.body.style.overflow = 'unset';
        };
    }, [isOpen, onClose]);

    const handleBackdropClick = (event: React.MouseEvent<HTMLDivElement>) => {
        if (
            modalRef.current &&
            !modalRef.current.contains(event.target as Node)
        ) {
            onClose();
        }
    };

    if (!isOpen) return null;

    return (
        <div className={styles.overlay} onClick={handleBackdropClick}>
            <div
                ref={modalRef}
                className={styles.modal}
                style={{
                    width: `${width}%`,
                    height: height ? `${height}%` : 'auto',
                }}
            >
                <button
                    className={styles.closeButton}
                    onClick={onClose}
                    aria-label='Закрыть модальное окно'
                >
                    ×
                </button>
                <div className={styles.content}>{children}</div>
            </div>
        </div>
    );
}
