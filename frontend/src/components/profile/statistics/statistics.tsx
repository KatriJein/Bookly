import styles from './statistics.module.scss';
import Icon from '../../../assets/svg/statistic.svg';

interface StatisticsProps {
    text: string;
    color1: string;
    color2: string;
}

export function Statistics({ text, color1, color2 }: StatisticsProps) {
    const gradientStyle = {
        background: `linear-gradient(90deg, ${color1} 0%, ${color2} 100%)`,
    };

    return (
        <div className={styles.statistics} style={gradientStyle}>
            <img src={Icon} alt='Icon' className={styles.icon} />
            <p className={styles.text}>{text}</p>
        </div>
    );
}
