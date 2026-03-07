export interface CreateDayDto {
    date: string;
    basePrice: number;
}

export interface UpdateDayDto {
    date: string;
    basePrice: number;
}

export interface GetDayDto {
    id: number;
    date: string;
    basePrice: number;
}