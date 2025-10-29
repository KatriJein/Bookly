interface IconProps {
    className?: string;
}

export function MenuLike({ className }: IconProps) {
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
                d='M13.7503 2.5C12.3003 2.5 10.9087 3.175 10.0003 4.23333C9.09199 3.175 7.70033 2.5 6.25033 2.5C3.68366 2.5 1.66699 4.50833 1.66699 7.08333C1.66699 10.225 4.50033 12.8 8.79199 16.6917C8.79199 16.6917 9.3622 17.125 10.0003 17.125C10.6385 17.125 11.2087 16.6917 11.2087 16.6917C15.5003 12.8 18.3337 10.225 18.3337 7.08333C18.3337 4.50833 16.317 2.5 13.7503 2.5Z'
                stroke='currentColor'
                stroke-width='1.5'
            />
        </svg>
    );
}
