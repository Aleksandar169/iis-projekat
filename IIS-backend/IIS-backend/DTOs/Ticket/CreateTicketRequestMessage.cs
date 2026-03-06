namespace IIS_backend.DTOs.Ticket;

public class CreateTicketRequestMessage
{
    public string TicketCode { get; set; } = string.Empty;

    public CreateTicketDto Payload { get; set; } = new();
}