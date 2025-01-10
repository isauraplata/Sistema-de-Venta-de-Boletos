namespace TicketingSystem.DTOs.Ticket
{
    public class TicketResponseDTO
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; } 
        public string Type { get; set; }
        public decimal Price { get; set; }
        public bool IsUsed { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string SeatNumber { get; set; } 

        public string QrCode { get; set; } 
    }
}
