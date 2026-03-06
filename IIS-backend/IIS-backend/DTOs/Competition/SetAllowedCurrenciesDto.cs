using System.Collections.Generic;

namespace IIS_backend.DTOs.Competition;

public class SetAllowedCurrenciesDto
{
    public List<long> CurrencyIds { get; set; } = new();
}