interface IconProps {
    className?: string;
}

export function MenuList({ className }: IconProps) {
    return (
        <svg
            className={className}
            width='20'
            height='20'
            viewBox='0 0 20 20'
            fill='none'
            xmlns='http://www.w3.org/2000/svg'
        >
            <path
                d='M11.667 13.3333L13.417 15.4167L16.667 11.25'
                stroke='currentColor'
                stroke-width='1.5'
                stroke-linecap='round'
                stroke-linejoin='round'
            />
            <path
                d='M17.5 5H2.5M17.5 8.33333H2.5M8.33333 11.6667H2.5M8.33333 15H2.5'
                stroke='currentColor'
                stroke-width='1.5'
                stroke-linecap='round'
            />
        </svg>
    );
}
