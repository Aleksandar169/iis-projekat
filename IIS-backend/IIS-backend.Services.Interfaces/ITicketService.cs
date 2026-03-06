using IIS_backend.Domain.Entities;

namespace IIS_backend.Services.Interfaces;

public interface ITicketService
{
    Task<Ticket?> GetByEmailAndCode(string email, string ticketCode);
    Task<TicketCreationRequestError?> GetError(string email, string ticketCode);

    Task<Ticket> CreateTicket(
        Ticket ticket,
        List<TicketSelection> selections,
        string? promoCode,
        long selectedCurrencyId
    );

    Task<Ticket> AddSelections(string email, string ticketCode, List<TicketSelection> add);
    Task<Ticket> RemoveSelections(string email, string ticketCode, List<long> dayIdsToRemove);
    Task<Ticket> Cancel(string email, string ticketCode);
}

public class TicketSelection
{
    public long DayId { get; set; }
    public long ZoneId { get; set; }
}