using TicketingSystem.DTOs.Ticket;

namespace TicketingSystem.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketResponseDTO>> GetAllTicketsAsync();
        Task<TicketResponseDTO> GetTicketByUuidAsync(Guid uuid);
        Task<TicketResponseDTO> CreateTicketAsync(TicketCreateDTO ticketDto);
        Task<bool> UpdateTicketAsync(Guid uuid, TicketCreateDTO ticketDto);
        Task<bool> DeleteTicketAsync(Guid uuid);
    }
}
