using TicketingSystem.Models;

namespace TicketingSystem.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket> GetByUuidAsync(Guid uuid);
        Task<List<Ticket>> GetAllAsync();
        Task<Ticket> CreateAsync(Ticket entity);
        Task<bool> UpdateAsync(Guid uuid, Ticket entity);
        Task<bool> DeleteAsync(Guid uuid);

    }
}
