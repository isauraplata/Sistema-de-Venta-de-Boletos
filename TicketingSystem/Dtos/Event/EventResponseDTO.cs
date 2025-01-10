namespace TicketingSystem.DTOs.Event
{
    public class EventResponseDTO
    {
         public Guid Uuid { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }

        public string ImagePath { get; set; } 
    }
}
