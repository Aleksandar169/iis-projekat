namespace IIS_backend.DTOs.Competition;
using System.Collections.Generic;
using System;

public class CompetitionBasicInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DiscountValidUntil { get; set; }
    public string? AdditionalInfo { get; set; }

    public List<DayDto> Days { get; set; } = new();
    public List<ZoneDto> Zones { get; set; } = new();
    public List<CurrencyDto> AllowedCurrencies { get; set; } = new();
}

public class DayDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal BasePrice { get; set; }
}

public class ZoneDto
{
    public long Id { get; set; }
    public int Capacity { get; set; }
    public string Characteristics { get; set; } = string.Empty;
    public decimal PriceAddon { get; set; }
}

public class CurrencyDto
{
    public long Id { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}