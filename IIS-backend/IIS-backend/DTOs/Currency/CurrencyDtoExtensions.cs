using CurrencyEntity= IIS_backend.Domain.Entities.Currency;

namespace IIS_backend.DTOs.Currency;

public static class CurrencyDtoExtensions
{
    public static CurrencyDto ToDto(this CurrencyEntity c) => new()
    {
        Id = c.Id,
        CurrencyName = c.CurrencyName,
        Code = c.Code
    };
}