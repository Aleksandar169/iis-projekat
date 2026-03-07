import { apiRequest } from './apiClient';
import type {
    CompetitionBasicInfoDto,
    SetAllowedCurrenciesDto,
    UpdateCompetitionDto,
} from '../model/competition';
import type { CurrencyDto } from '../model/currency';

export const competitionService = {
    async getCompetition(): Promise<CompetitionBasicInfoDto> {
        return apiRequest<CompetitionBasicInfoDto>('/Competition');
    },

    async tryGetCompetition(): Promise<CompetitionBasicInfoDto | null> {
        try {
            return await apiRequest<CompetitionBasicInfoDto>('/Competition');
        } catch {
            return null;
        }
    },

    async updateCompetition(dto: UpdateCompetitionDto): Promise<CompetitionBasicInfoDto> {
        return apiRequest<CompetitionBasicInfoDto>('/Competition', {
            method: 'PUT',
            body: JSON.stringify(dto),
        });
    },

    async setAllowedCurrencies(dto: SetAllowedCurrenciesDto): Promise<number[]> {
        return apiRequest<number[]>('/Competition/allowed-currencies', {
            method: 'PUT',
            body: JSON.stringify(dto),
        });
    },

    async getAllowedCurrencies(): Promise<CurrencyDto[]> {
        return apiRequest<CurrencyDto[]>('/Competition/allowed-currencies');
    },
};