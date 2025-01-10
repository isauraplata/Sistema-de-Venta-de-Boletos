using TicketingSystem.DTOs.Payment;
namespace TicketingSystem.Interfaces
{
    public interface IPaypalService
    {
        Task<string> GetAccessTokenAsync();
        Task<PayPalPaymentResponseDTO> CreateOrderAsync(PayPalPaymentCreateDTO paymentDto, string userId);
        Task<PayPalPaymentResponseDTO> CaptureOrderAsync(string orderId);
    }
}

