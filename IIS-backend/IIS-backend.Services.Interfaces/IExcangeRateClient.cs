namespace IIS_backend.Services.Interfaces;

public interface IExchangeRateClient
{
    /// <summary>
    /// Vraća kurs BASE->target (npr EUR->RSD)
    /// </summary>
    Task<decimal> GetRateAsync(string targetCurrencyCode);
}