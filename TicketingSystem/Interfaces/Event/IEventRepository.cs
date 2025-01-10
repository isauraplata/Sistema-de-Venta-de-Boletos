using TicketingSystem.Models;

namespace TicketingSystem.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> GetByUuidAsync(Guid uuid);
        Task<List<Event>> GetAllAsync();
        Task<Event> CreateAsync(Event entity);
        Task<bool> UpdateAsync(Guid uuid, Event entity);
        Task<bool> DeleteAsync(Guid uuid);
        Task<bool> ExistsAsync(Guid uuid);
        Task<bool> HasAvailableTicketsAsync(Guid uuid);
    }
}