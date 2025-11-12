interface HeartIconProps {
    className?: string;
    strokeClassName?: string;
    size?: number;
}

export function HeartIcon({
    className = '',
    size = 34,
    strokeClassName = '',
}: HeartIconProps) {
    return (
        <svg
            className={className}
            width={size}
            height={size}
            viewBox='0 0 34 34'
            fill='none'
            xmlns='http://www.w3.org/2000/svg'
        >
            <path
                d='M22.8016 4.14575C20.3971 4.14575 18.0892 5.26511 16.5829 7.02016C15.0766 5.26511 12.7688 4.14575 10.3643 4.14575C6.10794 4.14575 2.76367 7.4762 2.76367 11.7464C2.76367 16.9562 7.46222 21.2264 14.5791 27.68C14.5791 27.68 15.5247 28.3986 16.5829 28.3986C17.6412 28.3986 18.5867 27.68 18.5867 27.68C25.7037 21.2264 30.4022 16.9562 30.4022 11.7464C30.4022 7.4762 27.0579 4.14575 22.8016 4.14575Z'
                fill='currentColor'
                stroke='currentColor'
                strokeWidth='2'
                className={strokeClassName}
            />
        </svg>
    );
}
