namespace TicketingSystem.DTOs.Event
{
    public class EventCreateDTO
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int TotalTickets { get; set; }
         public IFormFile Image { get; set; } 
    }
}
