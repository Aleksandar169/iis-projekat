namespace IIS_backend.Services.Interfaces;

public interface IExchangeRateClient
{

    Task<decimal> GetRateAsync(string targetCurrencyCode);
}