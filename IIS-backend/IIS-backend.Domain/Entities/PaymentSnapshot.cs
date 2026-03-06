namespace IIS_backend.Domain.Entities;

public class PaymentSnapshot
{
    public long Id { get; set; }

    /// <summary>
    /// Kurs koji je korišćen u trenutku obračuna (1 BASE = ExchangeRate target valute ili obrnuto, vidi service)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    public decimal FinalAmount { get; set; }

    public long SelectedCurrencyId { get; set; }
    public Currency SelectedCurrency { get; set; } = null!;
}