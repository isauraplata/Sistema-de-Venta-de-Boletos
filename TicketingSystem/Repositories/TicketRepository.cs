using Microsoft.EntityFrameworkCore;
using TicketingSystem.Context;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByUuidAsync(Guid uuid)
        {
            return await _context.Ticket
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Uuid == uuid);
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Ticket
                .Include(t => t.Event)
                .ToListAsync();
        }

        public async Task<Ticket> CreateAsync(Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                await _context.Ticket.AddAsync(entity);
                await _context.SaveChangesAsync();

                return await _context.Ticket
                    .Include(t => t.Event)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Uuid == entity.Uuid);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al crear el ticket en la base de datos", ex);
            }
        }

        public async Task<bool> UpdateAsync(Guid uuid, Ticket entity)
        {
            var existingTicket = await GetByUuidAsync(uuid);
            if (existingTicket == null) return false;

            _context.Entry(existingTicket).CurrentValues.SetValues(entity);
            entity.Id = existingTicket.Id;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var ticket = await GetByUuidAsync(uuid);
            if (ticket == null) return false;

            _context.Ticket.Remove(ticket);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
