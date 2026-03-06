using IIS_backend.Domain.Entities;
using TicketEntity= IIS_backend.Domain.Entities.Ticket;
using IIS_backend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IIS_backend.DTOs.Ticket;

public static class TicketDtoExtensions
{
    public static TicketEntity ToTicketEntity(this CreateTicketDto dto, string ticketCode)
    {
        if (!dto.Email.Equals(dto.ConfirmEmail, StringComparison.OrdinalIgnoreCase))
            throw new Exception("Email i potvrda email adrese se ne poklapaju.");

        return new TicketEntity
        {
            TicketCode = ticketCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            AddressLine = dto.AddressLine,
            PostalCode = dto.PostalCode,
            City = dto.City,
            Country = dto.Country
        };
    }

    public static List<TicketSelection> ToSelections(this List<TicketSelectionDto> dto)
    {
        return dto.Select(x => new TicketSelection { DayId = x.DayId, ZoneId = x.ZoneId }).ToList();
    }

    public static GetTicketDto ToDto(this TicketEntity t)
    {
        return new GetTicketDto
        {
            Id = t.Id,
            TicketCode = t.TicketCode,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            City = t.City,
            Country = t.Country,
            AddressLine = t.AddressLine,
            PostalCode = t.PostalCode,
            PurchaseDate = t.PurchaseDate,
            Status = t.Status.ToString(),
            Items = t.TicketItems.Select(i => new GetTicketItemDto
            {
                DayId = i.DayId,
                DayDate = i.Day.Date,
                BasePrice = i.Day.BasePrice,
                ZoneId = i.ZoneId,
                ZoneCharacteristics = i.Zone.Characteristics,
                ZoneAddon = i.Zone.PriceAddon
            }).OrderBy(x => x.DayDate).ToList(),
            FinalAmount = t.PaymentSnapshot.FinalAmount,
            ExchangeRate = t.PaymentSnapshot.ExchangeRate,
            CurrencyCode = t.PaymentSnapshot.SelectedCurrency.Code,
            IssuedPromoCode = t.IssuedPromoCode.Code,
            UsedPromoCode = t.UsedPromoCode?.Code
        };
    }

    public static TicketErrorDto ToErrorDto(this TicketCreationRequestError e)
    {
        return new TicketErrorDto
        {
            Email = e.Email,
            TicketCode = e.TicketCode,
            ErrorMessage = e.ErrorMessage
        };
    }
}