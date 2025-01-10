using TicketingSystem.Interfaces;
using TicketingSystem.Models;
using TicketingSystem.DTOs.Ticket;
using QRCoder;
using System.Text.Json;

namespace TicketingSystem.Services;
public class QrCodeService : IQrCodeService
{
    private readonly ITicketRepository _ticketRepository;

    public QrCodeService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }


    public string GenerateQrCode(Ticket ticket)
    {
        var qrData = new TicketQrData
        {
            TicketId = ticket.Uuid,
            EventId = ticket.EventId,
            UserId = ticket.UserId,
            ValidUntil = ticket.ValidUntil,            
        };

        using var qrGenerator = new QRCodeGenerator();
        var jsonData = JsonSerializer.Serialize(qrData);
        using var qrCodeData = qrGenerator.CreateQrCode(jsonData, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new Base64QRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

        private string GenerateTicketHash(Ticket ticket)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var ticketData = $"{ticket.Uuid}{ticket.EventId}{ticket.UserId}{ticket.ValidUntil}";
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(ticketData));
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<bool> ValidateQrCode(string qrCode)
    {
        try
        {
            byte[] decodedBytes = Convert.FromBase64String(qrCode);
            string jsonData = System.Text.Encoding.UTF8.GetString(decodedBytes);
            var ticketData = JsonSerializer.Deserialize<TicketQrData>(jsonData);

            var ticket = await _ticketRepository.GetByUuidAsync(ticketData.TicketId);
            
            if (ticket == null) return false;
            if (ticket.IsUsed) return false;
            if (ticket.ValidUntil < DateTime.UtcNow) return false;
            
            ticket.IsUsed = true;
            await _ticketRepository.UpdateAsync(ticket.Uuid, ticket);
            
            return true;
        }
        catch
        {
            return false;
        }
    }


        public TicketQrData DecodeQrCode(string qrCode)
    {
        byte[] decodedBytes = Convert.FromBase64String(qrCode);
        string jsonData = System.Text.Encoding.UTF8.GetString(decodedBytes);
        return JsonSerializer.Deserialize<TicketQrData>(jsonData);
    }

     private bool IsTicketValid(Ticket ticket, TicketQrData ticketData)
    {
        if (ticket == null) return false;
        if (ticket.IsUsed) return false;
        if (ticket.ValidUntil < DateTime.UtcNow) return false;
        
        // Verificar el hash para prevenir manipulación
        var expectedHash = GenerateTicketHash(ticket);
        return ticketData.Hash == expectedHash;
    }


}