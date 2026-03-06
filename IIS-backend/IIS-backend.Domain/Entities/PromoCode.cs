namespace IIS_backend.Domain.Entities;

public enum PromoCodeStatus
{
    Active = 0,
    Inactive = 1,
    Used = 2
}

public class PromoCode
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public PromoCodeStatus Status { get; set; } = PromoCodeStatus.Active;

    public DateTime? UsageDate { get; set; }

    public Ticket IssuedTicket { get; set; } = null!;
    public Ticket? UsedTicket { get; set; }
}