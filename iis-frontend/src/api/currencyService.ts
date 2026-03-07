import { apiRequest } from './apiClient';
import type { CurrencyDto } from '../model/currency';

export const currencyService = {
    async getAll(): Promise<CurrencyDto[]> {
        return apiRequest<CurrencyDto[]>('/Currency');
    },
};