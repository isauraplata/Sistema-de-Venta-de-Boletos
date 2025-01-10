using TicketingSystem.DTOs.Ticket;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        private readonly IQrCodeService _qrCodeService;
        private readonly IEmailService _emailService;

        public TicketService(ITicketRepository ticketRepository, IQrCodeService qrCodeService, IEmailService emailService)
        {
            _ticketRepository = ticketRepository;
            _qrCodeService = qrCodeService;
            _emailService = emailService;
        }

        public async Task<List<TicketResponseDTO>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Select(MapToResponseDTO).ToList();
        }

        public async Task<TicketResponseDTO> GetTicketByUuidAsync(Guid uuid)
        {
            var ticket = await _ticketRepository.GetByUuidAsync(uuid);
            return ticket != null ? MapToResponseDTO(ticket) : null;
        }

        public async Task<TicketResponseDTO> CreateTicketAsync(TicketCreateDTO ticketDto)
        {
            var ticket = new Ticket
            {
                EventId = ticketDto.EventId,
                Type = ticketDto.Type,
                Price = ticketDto.Price,
                MaxPurchasePerUser = ticketDto.MaxPurchasePerUser,
                UserId = ticketDto.UserId,
                SeatNumber = ticketDto.SeatNumber,
                Uuid = Guid.NewGuid(),
                CreateAT = DateTime.UtcNow,
                UpdateAT = DateTime.UtcNow,
                ValidUntil = ticketDto.ValidUntil,
                IsUsed = false
            };

            ticket.QrCode = _qrCodeService.GenerateQrCode(ticket);
            var createdTicket = await _ticketRepository.CreateAsync(ticket);
            string qrCodeBase64 = _qrCodeService.GenerateQrCode(createdTicket);
            // Actualiza el ticket con el QR generado
            createdTicket.QrCode = qrCodeBase64;
            await _ticketRepository.UpdateAsync(createdTicket.Uuid, createdTicket);
            await _emailService.SendTicketEmailAsync(createdTicket, qrCodeBase64);
            return MapToResponseDTO(createdTicket);
        }

        public async Task<bool> UpdateTicketAsync(Guid uuid, TicketCreateDTO ticketDto)
        {
            var existingTicket = await _ticketRepository.GetByUuidAsync(uuid);
            if (existingTicket == null) return false;

            existingTicket.Type = ticketDto.Type;
            existingTicket.Price = ticketDto.Price;
            existingTicket.UpdateAT = DateTime.UtcNow;

            return await _ticketRepository.UpdateAsync(uuid, existingTicket);
        }

        public async Task<bool> DeleteTicketAsync(Guid uuid)
        {
            return await _ticketRepository.DeleteAsync(uuid);
        }

        private static TicketResponseDTO MapToResponseDTO(Ticket ticket)
        {
            return new TicketResponseDTO
            {
                Id = ticket.Id,
                Uuid = ticket.Uuid,
                Type = ticket.Type,
                Price = ticket.Price,
                IsUsed = ticket.IsUsed,
                EventId = ticket.EventId,
                EventName = ticket.Event?.Name ?? "Unknown",
                QrCode = ticket.QrCode
            };
        }
    }
}
