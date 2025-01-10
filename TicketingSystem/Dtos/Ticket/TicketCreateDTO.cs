namespace TicketingSystem.DTOs.Ticket
{
    public class TicketCreateDTO
    {
        public int EventId { get; set; } 
        public string Type { get; set; } // Tipo de ticket, e.g., "VIP"
        public decimal Price { get; set; } 
        public int MaxPurchasePerUser { get; set; } // Máximo de tickets por usuario
        public string UserId { get; set; } // ID del usuario que compra el ticket
        public string SeatNumber { get; set; } 
        public DateTime ValidUntil { get; set; }
    }
}
