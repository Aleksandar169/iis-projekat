namespace IIS_backend.Domain.Entities;

public class Currency
{
    public long Id { get; set; }
    public string CurrencyName { get; set; } = string.Empty;

    /// <summary>
    /// ISO code npr: RSD, EUR, USD
    /// </summary>
    public string Code { get; set; } = string.Empty;

    public List<CompetitionCurrency> CompetitionCurrencies { get; set; } = new();
}