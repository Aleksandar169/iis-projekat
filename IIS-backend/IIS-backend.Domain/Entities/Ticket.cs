namespace IIS_backend.Domain.Entities;

public enum TicketStatus
{
    Active = 0,
    Cancelled = 1
}

public class Ticket
{
    public long Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.Active;

    /// <summary>
    /// “Šifra” za kasniji pristup (random string)
    /// </summary>
    public string TicketCode { get; set; } = string.Empty;

    public List<TicketItem> TicketItems { get; set; } = new();

    public PaymentSnapshot PaymentSnapshot { get; set; } = null!;
    public long PaymentSnapshotId { get; set; }

    // Promo: izdata ovom ticket-u (obavezno)
    public long IssuedPromoCodeId { get; set; }
    public PromoCode IssuedPromoCode { get; set; } = null!;

    // Promo: iskorišćena na ovom ticket-u (opciono)
    public long? UsedPromoCodeId { get; set; }
    public PromoCode? UsedPromoCode { get; set; }
}