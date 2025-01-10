namespace TicketingSystem.DTOs.Payment
{
    public class PayPalPaymentCreateDTO
    {
        public string ItemName { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
    }

}