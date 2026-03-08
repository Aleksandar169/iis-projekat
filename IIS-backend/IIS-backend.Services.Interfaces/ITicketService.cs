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

    Task<TicketCalculationResult> CalculatePreview(
        List<TicketSelection> selections,
        string? promoCode,
        long selectedCurrencyId
    );
}

public class TicketSelection
{
    public long DayId { get; set; }
    public long ZoneId { get; set; }
}

public class TicketCalculationResult
{
    public List<TicketCalculationItem> Items { get; set; } = new();

    public decimal SubtotalRsd { get; set; }
    public bool DateDiscountApplied { get; set; }
    public decimal DateDiscountAmountRsd { get; set; }

    public bool PromoDiscountApplied { get; set; }
    public decimal PromoDiscountAmountRsd { get; set; }

    public decimal TotalAfterDiscountsRsd { get; set; }

    public long SelectedCurrencyId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public decimal FinalAmount { get; set; }
}

public class TicketCalculationItem
{
    public long DayId { get; set; }
    public DateTime DayDate { get; set; }
    public decimal BasePriceRsd { get; set; }

    public long ZoneId { get; set; }
    public string ZoneCharacteristics { get; set; } = string.Empty;
    public decimal ZoneAddonRsd { get; set; }

    public decimal ItemTotalRsd { get; set; }
}