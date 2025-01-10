using TicketingSystem.DTOs.Event;

namespace TicketingSystem.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDTO>> GetAllEventsAsync();
        Task<EventResponseDTO> GetEventByUuidAsync(Guid uuid);
        Task<EventResponseDTO> CreateEventAsync(EventCreateDTO eventDto);
        Task<bool> UpdateEventAsync(Guid uuid, EventUpdateDTO eventDto);
        Task<bool> DeleteEventAsync(Guid uuid);
        Task<bool> CheckAvailabilityAsync(Guid uuid);
    }
}