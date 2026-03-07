import { apiRequest } from './apiClient';
import type { CreateDayDto, GetDayDto, UpdateDayDto } from '../model/day';

export const dayService = {
    async getAll(): Promise<GetDayDto[]> {
        return apiRequest<GetDayDto[]>('/Day');
    },

    async getById(id: number): Promise<GetDayDto> {
        return apiRequest<GetDayDto>(`/Day/${id}`);
    },

    async create(dto: CreateDayDto): Promise<GetDayDto> {
        return apiRequest<GetDayDto>('/Day', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async update(id: number, dto: UpdateDayDto): Promise<GetDayDto> {
        return apiRequest<GetDayDto>(`/Day/${id}`, {
            method: 'PUT',
            body: JSON.stringify(dto),
        });
    },

    async delete(id: number): Promise<void> {
        return apiRequest<void>(`/Day/${id}`, {
            method: 'DELETE',
        });
    },
};