namespace IIS_backend.Domain.Entities;

public class PaymentSnapshot
{
    public long Id { get; set; }

    public decimal ExchangeRate { get; set; }

    public decimal FinalAmount { get; set; }

    public long SelectedCurrencyId { get; set; }
    public Currency SelectedCurrency { get; set; } = null!;
}