namespace IIS_backend.Domain.Entities;

public class Day
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal BasePrice { get; set; }

    public long CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public List<TicketItem> TicketItems { get; set; } = new();
}