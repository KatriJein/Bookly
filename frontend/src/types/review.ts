export type Review = {
    text: string;
    rating: number;
    userInfo: {
        id: string;
        login: string;
        avatarUrl: string;
    };
    createdAt: string;
    updatedAt: string;
};
