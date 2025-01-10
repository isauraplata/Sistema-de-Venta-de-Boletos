using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Models
{
    public class User : IdentityUser
    {

        [Required]
        public Guid Uuid { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public DateTime CreateAT { get; set; }
        public DateTime UpdateAT { get; set; }

    }

}
