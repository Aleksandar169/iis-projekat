using System;
using System.Collections.Generic;

namespace IIS_backend.DTOs.Ticket;

public class GetTicketDto
{
    public long Id { get; set; }
    public string TicketCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<GetTicketItemDto> Items { get; set; } = new();

    public decimal FinalAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;

    public string IssuedPromoCode { get; set; } = string.Empty;
    public string? UsedPromoCode { get; set; }
}

public class GetTicketItemDto
{
    public long DayId { get; set; }
    public DateTime DayDate { get; set; }
    public decimal BasePrice { get; set; }

    public long ZoneId { get; set; }
    public string ZoneCharacteristics { get; set; } = string.Empty;
    public decimal ZoneAddon { get; set; }
}