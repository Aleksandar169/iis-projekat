namespace IIS_backend.DTOs.Ticket;

public class CreateTicketResponseMessage
{
    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
}