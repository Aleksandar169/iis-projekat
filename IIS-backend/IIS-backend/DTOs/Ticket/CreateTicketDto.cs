using System.Collections.Generic;

namespace IIS_backend.DTOs.Ticket;

public class CreateTicketDto
{
  
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string ConfirmEmail { get; set; } = string.Empty;


    public List<TicketSelectionDto> Selections { get; set; } = new();


    public string? PromoCode { get; set; }


    public long SelectedCurrencyId { get; set; }
}

public class TicketSelectionDto
{
    public long DayId { get; set; }
    public long ZoneId { get; set; }
}