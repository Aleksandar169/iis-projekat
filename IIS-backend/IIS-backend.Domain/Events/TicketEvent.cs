using System;

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
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TicketUpdatedEventData
    {
        public long TicketId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TicketCancelledEventData
    {
        public long TicketId { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}