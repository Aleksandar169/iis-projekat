using System.Collections.Generic;

namespace IIS_backend.DTOs.Ticket;

public class ModifyTicketAddDto
{
    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;

    public List<TicketSelectionDto> Add { get; set; } = new();
}

public class ModifyTicketRemoveDto
{
    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;

    public List<long> RemoveDayIds { get; set; } = new();
}

public class CancelTicketDto
{
    public string Email { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
}