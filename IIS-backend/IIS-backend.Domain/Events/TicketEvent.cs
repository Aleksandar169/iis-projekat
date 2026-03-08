using System;
using System.Collections.Generic;

namespace IIS_backend.Domain.Events
{
    public class TicketEvent
    {
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Data { get; set; } = string.Empty;
    }

    public class TicketCreatedEventData
    {
        public long TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }

        public List<DateTime> CompetitionDays { get; set; } = new();
    }

    public class TicketUpdatedEventData
    {
        public long TicketId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<DateTime> CompetitionDays { get; set; } = new();
    }

    public class TicketCancelledEventData
    {
        public long TicketId { get; set; }
        public DateTime CancelledAt { get; set; }

        public List<DateTime> CompetitionDays { get; set; } = new();
    }
}