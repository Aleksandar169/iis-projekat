import { apiRequest } from './apiClient';
import type { CreateZoneDto, GetZoneDto, UpdateZoneDto } from '../model/zone';

export const zoneService = {
    async getAll(): Promise<GetZoneDto[]> {
        return apiRequest<GetZoneDto[]>('/Zone');
    },

    async getById(id: number): Promise<GetZoneDto> {
        return apiRequest<GetZoneDto>(`/Zone/${id}`);
    },

    async create(dto: CreateZoneDto): Promise<GetZoneDto> {
        return apiRequest<GetZoneDto>('/Zone', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async update(id: number, dto: UpdateZoneDto): Promise<GetZoneDto> {
        return apiRequest<GetZoneDto>(`/Zone/${id}`, {
            method: 'PUT',
            body: JSON.stringify(dto),
        });
    },

    async delete(id: number): Promise<void> {
        return apiRequest<void>(`/Zone/${id}`, {
            method: 'DELETE',
        });
    },
};