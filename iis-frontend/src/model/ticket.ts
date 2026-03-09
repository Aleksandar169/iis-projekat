export interface TicketSelectionDto {
    dayId: number;
    zoneId: number;
}

export interface CreateTicketDto {
    firstName: string;
    lastName: string;
    addressLine: string;
    postalCode: string;
    city: string;
    country: string;
    email: string;
    confirmEmail: string;
    selections: TicketSelectionDto[];
    promoCode?: string | null;
    selectedCurrencyId: number;
}

export interface CreateTicketResponseMessage {
    email: string;
    ticketCode: string;
}

export interface GetTicketItemDto {
    dayId: number;
    dayDate: string;
    basePrice: number;
    zoneId: number;
    zoneCharacteristics: string;
    zoneAddon: number;
}

export interface GetTicketDto {
    id: number;
    ticketCode: string;
    firstName: string;
    lastName: string;
    email: string;
    city: string;
    country: string;
    addressLine: string;
    postalCode: string;
    purchaseDate: string;
    status: string;
    items: GetTicketItemDto[];
    finalAmount: number;
    exchangeRate: number;
    currencyCode: string;
    issuedPromoCode: string;
    usedPromoCode?: string | null;
}

export interface TicketErrorDto {
    email: string;
    ticketCode: string;
    errorMessage: string;
}

export interface ModifyTicketAddDto {
    email: string;
    ticketCode: string;
    add: TicketSelectionDto[];
}

export interface ModifyTicketRemoveDto {
    email: string;
    ticketCode: string;
    removeDayIds: number[];
}

export interface CancelTicketDto {
    email: string;
    ticketCode: string;
}

export interface CalculateTicketDto {
    selections: TicketSelectionDto[];
    promoCode?: string | null;
    selectedCurrencyId: number;
    
}

export interface CalculatedTicketItemDto {
    dayId: number;
    dayDate: string;
    basePriceRsd: number;
    zoneId: number;
    zoneCharacteristics: string;
    zoneAddonRsd: number;
    itemTotalRsd: number;
}

export interface CalculateTicketResponseDto {
    items: CalculatedTicketItemDto[];
    subtotalRsd: number;
    dateDiscountApplied: boolean;
    dateDiscountAmountRsd: number;
    promoDiscountApplied: boolean;
    promoDiscountAmountRsd: number;
    totalAfterDiscountsRsd: number;
    selectedCurrencyId: number;
    currencyCode: string;
    exchangeRate: number;
    finalAmount: number;
}