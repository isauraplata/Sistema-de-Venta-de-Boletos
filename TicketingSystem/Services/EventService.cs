using TicketingSystem.DTOs.Event;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IFileService _fileService;

        public EventService(IEventRepository eventRepository,IFileService fileService)
        {
            _eventRepository = eventRepository;
            _fileService = fileService;
        }

        public async Task<List<EventResponseDTO>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return events.Select(MapToResponseDTO).ToList();
        }

        public async Task<EventResponseDTO> GetEventByUuidAsync(Guid uuid)
        {
            var @event = await _eventRepository.GetByUuidAsync(uuid);
            return @event != null ? MapToResponseDTO(@event) : null;
        }

    public async Task<EventResponseDTO> CreateEventAsync(EventCreateDTO eventDto)
    {
        string imagePath = null;
        if (eventDto.Image != null)
        {
            imagePath = await _fileService.SaveImageAsync(eventDto.Image);
        }

        var @event = new Event
        {
            Name = eventDto.Name,
            Date = eventDto.Date,
            Location = eventDto.Location,
            TotalTickets = eventDto.TotalTickets,
            Uuid = Guid.NewGuid(),
            ImageUrl = imagePath,
            CreateAT = DateTime.UtcNow,
            UpdateAT = DateTime.UtcNow
        };

        var createdEvent = await _eventRepository.CreateAsync(@event);
        return MapToResponseDTO(createdEvent);
    }

            public async Task<bool> UpdateEventAsync(Guid uuid, EventUpdateDTO eventDto)
    {
        var existingEvent = await _eventRepository.GetByUuidAsync(uuid);
        if (existingEvent == null) return false;

        if (eventDto.Image != null)
        {
            // Eliminar la imagen anterior si existe
            if (!string.IsNullOrEmpty(existingEvent.ImageUrl))
            {
                _fileService.DeleteImage(existingEvent.ImageUrl);
            }
            
            // Guardar la nueva imagen
            existingEvent.ImageUrl = await _fileService.SaveImageAsync(eventDto.Image);
        }

        existingEvent.Name = eventDto.Name;
        existingEvent.Date = eventDto.Date;
        existingEvent.Location = eventDto.Location;
        existingEvent.TotalTickets = eventDto.TotalTickets;
        existingEvent.UpdateAT = DateTime.UtcNow;

        return await _eventRepository.UpdateAsync(uuid, existingEvent);
    }

        public async Task<bool> DeleteEventAsync(Guid uuid)
        {
            return await _eventRepository.DeleteAsync(uuid);
        }

        public async Task<bool> CheckAvailabilityAsync(Guid uuid)
        {
            return await _eventRepository.HasAvailableTicketsAsync(uuid);
        }

        private static EventResponseDTO MapToResponseDTO(Event @event)
        {
            return new EventResponseDTO
            {
                Uuid = @event.Uuid,
                Name = @event.Name,
                Date = @event.Date,
                Location = @event.Location,
                ImagePath = @event.ImageUrl

            };
        }
    }
}