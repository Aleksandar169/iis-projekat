import { apiRequest } from './apiClient';
import type {
    CancelTicketDto,
    CalculateTicketDto,
    CalculateTicketResponseDto,
    CreateTicketDto,
    CreateTicketResponseMessage,
    GetTicketDto,
    ModifyTicketAddDto,
    ModifyTicketRemoveDto,
    TicketErrorDto,
} from '../model/ticket';

export const ticketService = {
    async getByEmailAndCode(email: string, ticketCode: string): Promise<GetTicketDto> {
        const params = new URLSearchParams({
            email,
            ticketCode,
        });

        return apiRequest<GetTicketDto>(`/Ticket?${params.toString()}`);
    },

    async getError(email: string, ticketCode: string): Promise<TicketErrorDto> {
        const params = new URLSearchParams({
            email,
            ticketCode,
        });

        return apiRequest<TicketErrorDto>(`/Ticket/error?${params.toString()}`);
    },

    async calculate(dto: CalculateTicketDto): Promise<CalculateTicketResponseDto> {
        return apiRequest<CalculateTicketResponseDto>('/Ticket/calculate', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async create(dto: CreateTicketDto): Promise<CreateTicketResponseMessage> {
        return apiRequest<CreateTicketResponseMessage>('/Ticket/create', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async add(dto: ModifyTicketAddDto): Promise<GetTicketDto> {
        return apiRequest<GetTicketDto>('/Ticket/add', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async remove(dto: ModifyTicketRemoveDto): Promise<GetTicketDto> {
        return apiRequest<GetTicketDto>('/Ticket/remove', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },

    async cancel(dto: CancelTicketDto): Promise<void> {
        return apiRequest<void>('/Ticket/cancel', {
            method: 'POST',
            body: JSON.stringify(dto),
        });
    },
};