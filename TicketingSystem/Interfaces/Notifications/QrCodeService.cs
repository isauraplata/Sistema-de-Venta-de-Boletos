namespace TicketingSystem.Interfaces;
using TicketingSystem.DTOs.Ticket;
using TicketingSystem.Models;

public interface IQrCodeService
{
    string GenerateQrCode(Ticket ticket);
    Task<bool> ValidateQrCode(string qrCode);

    TicketQrData DecodeQrCode(string qrCode);
}