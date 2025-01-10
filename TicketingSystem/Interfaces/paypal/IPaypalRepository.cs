using TicketingSystem.Models;

namespace TicketingSystem.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
        Task<Payment> GetByOrderIdAsync(string orderId);
        Task<List<Payment>> GetByUserIdAsync(string userId);
    }
}