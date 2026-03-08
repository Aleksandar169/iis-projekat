using System.Text.Json;
using IIS_backend.Domain.Events;
using IIS_backend.DTOs.Ticket;
using IIS_backend.Services.Interfaces;
using StackExchange.Redis;


namespace IIS_backend.HostedService
{
    public class BackgroundWorker : BackgroundService
    {
        public const string TicketCreateQueueName = "ticket_create_queue";
        public const string TicketEventsChannel = "ticket_events";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly ISubscriber _subscriber;

        public BackgroundWorker(
            IConnectionMultiplexer mux,
            IServiceScopeFactory scopeFactory,
            ILogger<BackgroundWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _subscriber = mux.GetSubscriber();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriber.SubscribeAsync(RedisChannel.Literal(TicketCreateQueueName), async (_, message) =>
            {
                if (!message.HasValue) return;

                try
                {
                    var request = JsonSerializer.Deserialize<CreateTicketRequestMessage>(message!);
                    if (request == null) return;

                    using var scope = _scopeFactory.CreateScope();
                    var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();

                    var ticket = request.Payload.ToTicketEntity(request.TicketCode);

                    var created = await ticketService.CreateTicket(
                        ticket,
                        request.Payload.Selections.ToSelections(),
                        request.Payload.PromoCode,
                        request.Payload.SelectedCurrencyId
                    );

                    var createdEvent = new TicketEvent
                    {
                        EventDate = DateTime.UtcNow,
                        EventType = "TicketCreated",
                        Data = JsonSerializer.Serialize(new TicketCreatedEventData
                        {
                            TicketId = created.Id,
                            TicketCode = created.TicketCode,
                            Email = created.Email,
                            Country = created.Country,
                            PurchaseDate = created.PurchaseDate,
                            CompetitionDays = created.TicketItems
                                .Select(x => x.Day.Date)
                                .OrderBy(x => x)
                                .ToList()
                        })
                    };

                    await _subscriber.PublishAsync(
                        RedisChannel.Literal(TicketEventsChannel),
                        JsonSerializer.Serialize(createdEvent)
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing ticket creation message: {Message}", message);
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}