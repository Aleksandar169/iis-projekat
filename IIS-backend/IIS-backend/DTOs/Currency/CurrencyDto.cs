namespace IIS_backend.DTOs.Currency;

public class CurrencyDto
{
    public long Id { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}