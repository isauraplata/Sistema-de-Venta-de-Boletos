namespace TicketingSystem.Interfaces;
using TicketingSystem.Models;
public interface IEmailService
{
    Task SendTicketEmailAsync(Ticket ticket,string qrCodeBase64);
}