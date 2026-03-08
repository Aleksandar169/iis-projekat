using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using IIS_backend.Services.Interfaces;

namespace IIS_backend.Services;

public class ExchangeRateApiSettings
{
    public const string SectionName = "KursAPI";
    public string BaseUrl { get; set; } = string.Empty;
}

public class KursTodayRateResponse
{
    public string Code { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Date_From { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Parity { get; set; }
    public decimal Cash_Buy { get; set; }
    public decimal Cash_Sell { get; set; }
    public decimal Exchange_Buy { get; set; }
    public decimal Exchange_Middle { get; set; }
    public decimal Exchange_Sell { get; set; }
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
        var code = targetCurrencyCode.ToUpperInvariant();

        if (code == "RSD")
            return 1m;

        var url = $"{_settings.BaseUrl}{code}/rates/today";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<KursTodayRateResponse>();
        if (data == null)
            throw new Exception("Kurs nije pronađen.");

        return data.Exchange_Middle;
    }
}