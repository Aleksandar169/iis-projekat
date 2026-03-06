using System.Text.Json;
using IIS_backend.Domain.Events;
using IIS_backend.DTOs.Ticket;
using IIS_backend.HostedService;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Threading.Tasks;
using System;

namespace IIS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ISubscriber _subscriber;

        public TicketController(ITicketService ticketService, IConnectionMultiplexer mux)
        {
            _ticketService = ticketService;
            _subscriber = mux.GetSubscriber();
        }

        [HttpGet]
        public async Task<GetTicketDto?> Get([FromQuery] string email, [FromQuery] string ticketCode)
        {
            var ticket = await _ticketService.GetByEmailAndCode(email, ticketCode);
            return ticket?.ToDto();
        }

        [HttpGet("error")]
        public async Task<TicketErrorDto?> GetError([FromQuery] string email, [FromQuery] string ticketCode)
        {
            var err = await _ticketService.GetError(email, ticketCode);
            return err?.ToErrorDto();
        }

        // Feature 3 (var C): prvo u queue
        [HttpPost("create")]
        public CreateTicketResponseMessage Create([FromBody] CreateTicketDto dto)
        {
            var ticketCode = Guid.NewGuid().ToString("N")[..8];

            var msg = new CreateTicketRequestMessage
            {
                TicketCode = ticketCode,
                Payload = dto
            };

            _subscriber.Publish(
                RedisChannel.Literal(BackgroundWorker.TicketCreateQueueName),
                JsonSerializer.Serialize(msg)
            );

            return new CreateTicketResponseMessage
            {
                Email = dto.Email,
                TicketCode = ticketCode
            };
        }

        // Feature 4.1 - dodavanje dana
        [HttpPost("add")]
        public async Task<GetTicketDto> Add([FromBody] ModifyTicketAddDto dto)
        {
            await _ticketService.AddSelections(dto.Email, dto.TicketCode, dto.Add.ToSelections());

            var reloaded = await _ticketService.GetByEmailAndCode(dto.Email, dto.TicketCode);
            if (reloaded == null) throw new Exception("Karta nije pronađena posle izmene.");

            var evt = new TicketEvent
            {
                EventDate = DateTime.UtcNow,
                EventType = "TicketUpdated",
                Data = JsonSerializer.Serialize(new TicketUpdatedEventData
                {
                    TicketId = reloaded.Id,
                    UpdatedAt = DateTime.UtcNow
                })
            };

            await _subscriber.PublishAsync(
                RedisChannel.Literal(BackgroundWorker.TicketEventsChannel),
                JsonSerializer.Serialize(evt)
            );

            return reloaded.ToDto();
        }

        // Feature 4.2 - uklanjanje dana
        [HttpPost("remove")]
        public async Task<GetTicketDto> Remove([FromBody] ModifyTicketRemoveDto dto)
        {
            await _ticketService.RemoveSelections(dto.Email, dto.TicketCode, dto.RemoveDayIds);

            var reloaded = await _ticketService.GetByEmailAndCode(dto.Email, dto.TicketCode);
            if (reloaded == null) throw new Exception("Karta nije pronađena posle izmene.");

            var evt = new TicketEvent
            {
                EventDate = DateTime.UtcNow,
                EventType = "TicketUpdated",
                Data = JsonSerializer.Serialize(new TicketUpdatedEventData
                {
                    TicketId = reloaded.Id,
                    UpdatedAt = DateTime.UtcNow
                })
            };

            await _subscriber.PublishAsync(
                RedisChannel.Literal(BackgroundWorker.TicketEventsChannel),
                JsonSerializer.Serialize(evt)
            );

            return reloaded.ToDto();
        }

        // Feature 5 - otkazivanje
        [HttpPost("cancel")]
        public async Task Cancel([FromBody] CancelTicketDto dto)
        {
            var cancelled = await _ticketService.Cancel(dto.Email, dto.TicketCode);

            var evt = new TicketEvent
            {
                EventDate = DateTime.UtcNow,
                EventType = "TicketCancelled",
                Data = JsonSerializer.Serialize(new TicketCancelledEventData
                {
                    TicketId = cancelled.Id,
                    CancelledAt = DateTime.UtcNow
                })
            };

            await _subscriber.PublishAsync(
                RedisChannel.Literal(BackgroundWorker.TicketEventsChannel),
                JsonSerializer.Serialize(evt)
            );
        }
    }
}