namespace IIS_backend.Domain.Entities;

public class CompetitionCurrency
{
    public long CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
}