namespace IIS_backend.Domain.Entities;

public class Competition
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DiscountValidUntil { get; set; }
    public string? AdditionalInfo { get; set; }

    public List<Day> Days { get; set; } = new();
    public List<Zone> Zones { get; set; } = new();
    public List<CompetitionCurrency> CompetitionCurrencies { get; set; } = new();
}