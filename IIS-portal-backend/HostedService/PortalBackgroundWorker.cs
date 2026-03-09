namespace IIS_portal_backend.HostedService
{
    using System.Text.Json;
    using IIS_portal_backend.DataAccess.IIS_portal_backend.Data;
    using IIS_portal_backend.Models;
    using StackExchange.Redis;

  
    public class TicketEvent
    {
        public string EventType { get; set; }
        public string Data { get; set; } 
    }

    public class TicketCreatedEventData
    {
        public int TicketId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<DateTime> CompetitionDays { get; set; }
    }

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

                    if (ticketEvent?.EventType == "TicketCreated")
                    {
                        var eventData = JsonSerializer.Deserialize<TicketCreatedEventData>(ticketEvent.Data);

                        if (eventData == null) return;

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();

              
                        foreach (var day in eventData.CompetitionDays)
                        {
                            db.TicketSales.Add(new TicketSale
                            {
                                TicketId = eventData.TicketId,
                                PurchaseDate = eventData.PurchaseDate,
                                CompetitionDay = day
                            });
                        }

                        await db.SaveChangesAsync();
                        _logger.LogInformation($"Uspešno obrađena karta ID: {eventData.TicketId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška prilikom obrade poruke iz Redisa");
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}