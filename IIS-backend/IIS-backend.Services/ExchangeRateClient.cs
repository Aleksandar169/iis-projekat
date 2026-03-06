using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using IIS_backend.Services.Interfaces;

namespace IIS_backend.Services;

public class ExchangeRateApiSettings
{
    public const string SectionName = "ExchangeRateApi";
    public string BaseUrl { get; set; } = string.Empty;
}

public class FrankfurterResponse
{
    public Dictionary<string, decimal> rates { get; set; } = new();
}

public class ExchangeRateClient : IExchangeRateClient
{
    private readonly HttpClient _httpClient;
    private readonly ExchangeRateApiSettings _settings;

    public ExchangeRateClient(HttpClient httpClient, IOptions<ExchangeRateApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<decimal> GetRateAsync(string targetCurrencyCode)
    {
        // BaseUrl npr: https://api.frankfurter.app/latest?from=EUR&to=
        var url = $"{_settings.BaseUrl}{targetCurrencyCode.ToUpperInvariant()}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<FrankfurterResponse>();
        if (data == null || data.rates == null || data.rates.Count == 0)
            throw new Exception("Kurs nije pronađen.");

        if (!data.rates.TryGetValue(targetCurrencyCode.ToUpperInvariant(), out var rate))
            throw new Exception("Kurs nije pronađen za traženu valutu.");

        // rate = 1 EUR = rate target
        return rate;
    }
}