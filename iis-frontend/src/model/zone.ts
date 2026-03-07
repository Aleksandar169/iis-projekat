export interface CreateZoneDto {
    capacity: number;
    characteristics: string;
    priceAddon: number;
}

export interface UpdateZoneDto {
    capacity: number;
    characteristics: string;
    priceAddon: number;
}

export interface GetZoneDto {
    id: number;
    capacity: number;
    characteristics: string;
    priceAddon: number;
}