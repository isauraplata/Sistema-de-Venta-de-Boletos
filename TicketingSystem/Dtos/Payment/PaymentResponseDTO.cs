namespace TicketingSystem.DTOs.Payment
{

    public class PayPalPaymentResponseDTO
    {
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
    }
}
