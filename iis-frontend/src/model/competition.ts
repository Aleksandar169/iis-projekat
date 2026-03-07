import type { CurrencyDto } from './currency';

export interface CompetitionDayDto {
    id: number;
    date: string;
    basePrice: number;
}

export interface CompetitionZoneDto {
    id: number;
    capacity: number;
    characteristics: string;
    priceAddon: number;
}

export interface CompetitionBasicInfoDto {
    name: string;
    location: string;
    startDate: string;
    endDate: string;
    discountValidUntil: string;
    additionalInfo?: string | null;
    days: CompetitionDayDto[];
    zones: CompetitionZoneDto[];
    allowedCurrencies: CurrencyDto[];
}

export interface UpdateCompetitionDto {
    name: string;
    location: string;
    startDate: string;
    endDate: string;
    discountValidUntil: string;
    additionalInfo?: string | null;
}

export interface SetAllowedCurrenciesDto {
    currencyIds: number[];
}