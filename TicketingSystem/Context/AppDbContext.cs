using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Models;

namespace TicketingSystem.Context
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Event> Event { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        public DbSet<Payment> Payment { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
                       builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });
        }
    }
}


