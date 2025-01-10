using Microsoft.EntityFrameworkCore;
using TicketingSystem.Context;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> GetByOrderIdAsync(string orderId)
        {
            return await _context.Payment
                .FirstOrDefaultAsync(p => p.PaypalOrderId == orderId);
        }

        public async Task<List<Payment>> GetByUserIdAsync(string userId)
        {
            return await _context.Payment
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}