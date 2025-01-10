using Microsoft.AspNetCore.Mvc;
using TicketingSystem.DTOs.Event;
using TicketingSystem.Interfaces;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventResponseDTO>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{uuid:guid}")]
        public async Task<ActionResult<EventResponseDTO>> GetEvent(Guid uuid)
        {
            var @event = await _eventService.GetEventByUuidAsync(uuid);
            if (@event == null) return NotFound();
            return Ok(@event);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponseDTO>> CreateEvent([FromForm] EventCreateDTO eventDto)
        {
            var createdEvent = await _eventService.CreateEventAsync(eventDto);
            return CreatedAtAction(
                nameof(GetEvent),
                new { uuid = createdEvent.Uuid },
                createdEvent);
        }

        [HttpPut("{uuid:guid}")]
        public async Task<ActionResult> UpdateEvent(Guid uuid, [FromForm] EventUpdateDTO eventDto)
        {
            var success = await _eventService.UpdateEventAsync(uuid, eventDto);
            if (!success) return NotFound();
            return NoContent();
        }


        [HttpDelete("{uuid:guid}")]
        public async Task<ActionResult> DeleteEvent(Guid uuid)
        {
            var success = await _eventService.DeleteEventAsync(uuid);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{uuid:guid}/availability")]
        public async Task<ActionResult<bool>> CheckAvailability(Guid uuid)
        {
            var hasAvailability = await _eventService.CheckAvailabilityAsync(uuid);
            return Ok(hasAvailability);
        }
    }
}