namespace IIS_backend.Domain.Entities;

public class TicketItem
{
    public long Id { get; set; }

    public long TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public long DayId { get; set; }
    public Day Day { get; set; } = null!;

    public long ZoneId { get; set; }
    public Zone Zone { get; set; } = null!;
}