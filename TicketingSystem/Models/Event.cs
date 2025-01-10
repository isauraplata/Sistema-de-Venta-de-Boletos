using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid Uuid { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        [Required]
        public int TotalTickets { get; set; }

        [Required]
        public int TicketsSold { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public DateTime CreateAT { get; set; }

        public DateTime UpdateAT { get; set; }

    }
}
