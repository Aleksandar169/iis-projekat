namespace IIS_backend.DTOs.Ticket;

public class TicketErrorDto
{
    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}