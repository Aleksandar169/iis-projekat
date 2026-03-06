namespace IIS_backend.Domain.Entities;

public class Zone
{
    public long Id { get; set; }
    public int Capacity { get; set; }
    public string Characteristics { get; set; } = string.Empty;
    public decimal PriceAddon { get; set; }

    public long CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public List<TicketItem> TicketItems { get; set; } = new();
}