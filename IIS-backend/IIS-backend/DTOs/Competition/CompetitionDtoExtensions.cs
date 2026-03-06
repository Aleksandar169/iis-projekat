
using CompetitionEntity = IIS_backend.Domain.Entities.Competition;
using System.Collections.Generic;
using System.Linq;
using CurrencyEntity = IIS_backend.Domain.Entities.Currency;

namespace IIS_backend.DTOs.Competition;

public static class CompetitionDtoExtensions
{
    public static CompetitionBasicInfoDto ToBasicInfoDto(this CompetitionEntity c, List<CurrencyEntity> allowed)
    {
        return new CompetitionBasicInfoDto
        {
            Name = c.Name,
            Location = c.Location,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            DiscountValidUntil = c.DiscountValidUntil,
            AdditionalInfo = c.AdditionalInfo,
            Days = c.Days.OrderBy(d => d.Date).Select(d => new DayDto
            {
                Id = d.Id,
                Date = d.Date,
                BasePrice = d.BasePrice
            }).ToList(),
            Zones = c.Zones.Select(z => new ZoneDto
            {
                Id = z.Id,
                Capacity = z.Capacity,
                Characteristics = z.Characteristics,
                PriceAddon = z.PriceAddon
            }).ToList(),
            AllowedCurrencies = allowed.Select(cur => new CurrencyDto
            {
                Id = cur.Id,
                CurrencyName = cur.CurrencyName,
                Code = cur.Code
            }).ToList()
        };
    }

    public static CompetitionEntity ToEntity(this UpdateCompetitionDto dto)
    {
        return new CompetitionEntity
        {
            Name = dto.Name,
            Location = dto.Location,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DiscountValidUntil = dto.DiscountValidUntil,
            AdditionalInfo = dto.AdditionalInfo
        };
    }
}