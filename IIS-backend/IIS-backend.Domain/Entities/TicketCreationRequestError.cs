namespace IIS_backend.Domain.Entities;

public class TicketCreationRequestError
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}