using Microsoft.AspNetCore.Mvc;
using TicketingSystem.DTOs.Payment;
using TicketingSystem.Interfaces;
using System.Security.Claims;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaypalService _paypalService;

        public PaymentController(IPaypalService paypalService)
        {
            _paypalService = paypalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] PayPalPaymentCreateDTO paymentDto)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _paypalService.CreateOrderAsync(paymentDto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating order", error = ex.Message });
            }
        }

        [HttpPost("capture/{orderId}")]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            try
            {
                var result = await _paypalService.CaptureOrderAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error capturing order", error = ex.Message });
            }
        }

        [HttpGet("success")]
        public IActionResult Success([FromQuery] string token, [FromQuery] string PayerID)
        {
            return Ok(new { message = "Payment successful", token, PayerID });
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return Ok(new { message = "Payment cancelled" });
        }
    }
}