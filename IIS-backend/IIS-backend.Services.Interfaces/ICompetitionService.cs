using IIS_backend.Domain.Entities;

namespace IIS_backend.Services.Interfaces;

public interface ICompetitionService
{
    Task<Competition?> GetDetails();
    Task<Competition> Upsert(Competition competition);
    Task SetAllowedCurrencies(List<long> currencyIds);
    Task<List<Currency>> GetAllowedCurrencies();
}