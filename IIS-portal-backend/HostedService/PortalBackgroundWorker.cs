using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using IIS_portal_backend.DataAccess.IIS_portal_backend.Data;

namespace IIS_portal_backend.HostedService
{
    public class PortalBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISubscriber _subscriber;
        private readonly ILogger<PortalBackgroundWorker> _logger;

        public PortalBackgroundWorker(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory, ILogger<PortalBackgroundWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _subscriber = redis.GetSubscriber();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriber.SubscribeAsync(RedisChannel.Literal("ticket_events"), async (_, message) =>
            {
                if (!message.HasValue) return;

                try
                {
                    var ticketEvent = JsonSerializer.Deserialize<TicketEvent>(message.ToString());
                    if (ticketEvent == null) return;

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();

                    if (ticketEvent.EventType == "TicketCreated" || ticketEvent.EventType == "TicketUpdated")
                    {
                        var eventData = JsonSerializer.Deserialize<UniversalTicketData>(ticketEvent.Data);
                        if (eventData == null) return;

                       
                        var existingEntries = await db.TicketSales
                            .Where(t => t.TicketId == eventData.TicketId)
                            .ToListAsync();

                   
                        DateTime originalPurchaseDate = eventData.PurchaseDate ??
                                                      (existingEntries.FirstOrDefault()?.PurchaseDate ?? DateTime.UtcNow);

                        if (existingEntries.Any())
                        {
                            db.TicketSales.RemoveRange(existingEntries);
                            await db.SaveChangesAsync();
                        }

                        foreach (var day in eventData.CompetitionDays)
                        {
                            db.TicketSales.Add(new TicketSale
                            {
                                TicketId = eventData.TicketId,
                                PurchaseDate = originalPurchaseDate,
                                CompetitionDay = day
                            });
                        }
                        await db.SaveChangesAsync();
                        _logger.LogInformation($"Uspešno obrađen {ticketEvent.EventType} za ID: {eventData.TicketId}");
                    }

                
                    else if (ticketEvent.EventType == "TicketCancelled")
                    {
                        var eventData = JsonSerializer.Deserialize<UniversalTicketData>(ticketEvent.Data);
                        if (eventData == null) return;

                        var toRemove = await db.TicketSales.Where(t => t.TicketId == eventData.TicketId).ToListAsync();
                        if (toRemove.Any())
                        {
                            db.TicketSales.RemoveRange(toRemove);
                            await db.SaveChangesAsync();
                            _logger.LogInformation($"Karta ID {eventData.TicketId} izbrisana zbog otkazivanja.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška u obradi Redis poruke");
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        
        public class TicketEvent
        {
            public string EventType { get; set; }
            public string Data { get; set; }
        }

        public class UniversalTicketData
        {
            public int TicketId { get; set; }
            public DateTime? PurchaseDate { get; set; }

            public List<DateTime> CompetitionDays { get; set; } = new List<DateTime>();
        }
    }
}