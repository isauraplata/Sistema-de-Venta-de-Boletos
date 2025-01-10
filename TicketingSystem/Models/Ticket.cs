using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketingSystem.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid Uuid { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsUsed { get; set; } = false;

        public int MaxPurchasePerUser { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string QrCode { get; set; }

        public string SeatNumber { get; set; }

        public DateTime ValidUntil { get; set; }
        
        public DateTime CreateAT { get; set; }

        public DateTime UpdateAT { get; set; }
    }

}
