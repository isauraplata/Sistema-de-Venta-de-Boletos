using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TicketingSystem.DTOs.Payment;
using TicketingSystem.Interfaces;
using TicketingSystem.Models;

namespace TicketingSystem.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly string _clientId;
        private readonly string _secret;
        private readonly string _baseUrl;
        private readonly string _returnUrl;
        private readonly string _cancelUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPaymentRepository _paymentRepository;

        public PaypalService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IPaymentRepository paymentRepository)
        {
            _clientId = configuration["PayPalSettings:ClientId"]!;
            _secret = configuration["PayPalSettings:Secret"]!;
            _baseUrl = configuration["PayPalSettings:URL"]!;
            _returnUrl = configuration["PayPalSettings:ReturnUrl"]!;
            _cancelUrl = configuration["PayPalSettings:CancelUrl"]!;
            _httpClientFactory = httpClientFactory;
            _paymentRepository = paymentRepository;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            string url = $"{_baseUrl}/v1/oauth2/token";
            
            string credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));
            
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    "grant_type=client_credentials",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonNode>();
            return jsonResponse?["access_token"]?.ToString() ?? "";
        }

        public async Task<PayPalPaymentResponseDTO> CreateOrderAsync(
            PayPalPaymentCreateDTO paymentDto, string userId)
        {
            var accessToken = await GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            string url = $"{_baseUrl}/v2/checkout/orders";

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var orderRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = paymentDto.Currency,
                            value = paymentDto.Price.ToString("0.00")
                        },
                        description = paymentDto.Description
                    }
                },
                application_context = new
                {
                    return_url = _returnUrl,
                    cancel_url = _cancelUrl
                }
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(orderRequest),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();

            var orderResponse = await response.Content.ReadFromJsonAsync<JsonNode>();
            var orderId = orderResponse?["id"]?.ToString();

            var payment = new Payment
            {
                PaypalOrderId = orderId!,
                Amount = paymentDto.Price,
                Status = "CREATED",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);

            return new PayPalPaymentResponseDTO
            {
                PaymentId = orderId!,
                Status = "CREATED",
                RedirectUrl = orderResponse?["links"]?[1]?["href"]?.ToString() ?? ""
            };
        }

        public async Task<PayPalPaymentResponseDTO> CaptureOrderAsync(string orderId)
        {
            var accessToken = await GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            string url = $"{_baseUrl}/v2/checkout/orders/{orderId}/capture";

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment != null)
            {
                payment.Status = "COMPLETED";
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
            }

            return new PayPalPaymentResponseDTO
            {
                PaymentId = orderId,
                Status = "COMPLETED",
                RedirectUrl = ""
            };
        }
    }
}