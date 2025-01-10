namespace TicketingSystem.DTOs.Ticket;
public class TicketQrData
{
    public Guid TicketId { get; set; }
    public int EventId { get; set; }
    public string UserId { get; set; }
    public DateTime ValidUntil { get; set; }

    public string Hash { get; set; }
    public string SeatNumber { get; set; }
}


