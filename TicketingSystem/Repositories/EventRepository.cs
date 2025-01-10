using Microsoft.EntityFrameworkCore;
using TicketingSystem.Context;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> GetByUuidAsync(Guid uuid)
        {
            return await _context.Event
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Uuid == uuid);
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Event
                .Include(e => e.Tickets)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Event> CreateAsync(Event entity)
        {
            await _context.Event.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Guid uuid, Event entity)
        {
            var existingEvent = await GetByUuidAsync(uuid);
            if (existingEvent == null) return false;

            _context.Entry(existingEvent).CurrentValues.SetValues(entity);
            entity.Id = existingEvent.Id; // Mantenemos el mismo ID interno
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var entity = await GetByUuidAsync(uuid);
            if (entity == null) return false;

            _context.Event.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Guid uuid)
        {
            return await _context.Event.AnyAsync(e => e.Uuid == uuid);
        }

        public async Task<bool> HasAvailableTicketsAsync(Guid uuid)
        {
            var @event = await GetByUuidAsync(uuid);
            return @event != null && @event.TicketsSold < @event.TotalTickets;
        }

        // Métodos adicionales que podrían ser útiles
        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Event
                .Where(e => e.Date > DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByLocationAsync(string location)
        {
            return await _context.Event
                .Where(e => e.Location.Contains(location))
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<bool> UpdateTicketsSoldAsync(Guid uuid)
        {
            var @event = await GetByUuidAsync(uuid);
            if (@event == null) return false;

            @event.TicketsSold++;
            
            return await _context.SaveChangesAsync() > 0;
        }
    }
}