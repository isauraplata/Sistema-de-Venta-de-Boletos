using System.Net;
using System.Net.Mail;
using TicketingSystem.Models;
using TicketingSystem.Interfaces;


namespace TicketingSystem.Services;
public class EmailService : IEmailService
{
    private readonly string _gmailUser;
    private readonly string _gmailPassword;
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _logger = logger;
        _gmailUser = configuration["Email:GmailUser"];
        _gmailPassword = configuration["Email:GmailAppPassword"];
        _smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(_gmailUser, _gmailPassword),
            EnableSsl = true
        };
    }

    public async Task SendTicketEmailAsync(Ticket ticket, string qrCodeBase64)
    {

        Console.WriteLine("email: " + ticket.User.Email);
        
        if (ticket.User?.Email == null)
        {
            throw new InvalidOperationException($"Email no encontrado para el ticket {ticket.Uuid}");
        }

        try
        {
            var mailMessage = CreateMailMessage(ticket, qrCodeBase64);
            await _smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email enviado exitosamente para el ticket {TicketUuid}", ticket.Uuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email para el ticket {TicketUuid}", ticket.Uuid);
            throw;
        }
    }

    private MailMessage CreateMailMessage(Ticket ticket, string qrCodeBase64)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_gmailUser, "Sistema de Tickets"),
            Subject = $"Tu entrada para {ticket.Event?.Name ?? "el evento"}",
            Body = GenerateEmailTemplate(ticket, qrCodeBase64),
            IsBodyHtml = true
        };
        mailMessage.To.Add(ticket.User.Email);
        return mailMessage;
    }

    private string GenerateEmailTemplate(Ticket ticket, string qrCodeBase64)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
                    .ticket-container {{
                        max-width: 600px;
                        margin: 0 auto;
                        font-family: Arial, sans-serif;
                        background-color: #f9f9f9;
                        padding: 20px;
                        border-radius: 10px;
                    }}
                    .ticket-header {{
                        background-color: #4a90e2;
                        color: white;
                        padding: 20px;
                        text-align: center;
                        border-radius: 10px 10px 0 0;
                    }}
                    .ticket-content {{
                        background-color: white;
                        padding: 20px;
                        border-radius: 0 0 10px 10px;
                        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                    }}
                    .qr-container {{
                        text-align: center;
                        margin: 20px 0;
                    }}
                    .ticket-info {{
                        margin: 15px 0;
                        padding: 10px;
                        border-left: 4px solid #4a90e2;
                    }}
                </style>
            </head>
            <body>
                <div class='ticket-container'>
                    <div class='ticket-header'>
                        <h1>{ticket.Event?.Name}</h1>
                    </div>
                    <div class='ticket-content'>
                        <div class='ticket-info'>
                            <p><strong>Fecha:</strong> {ticket.Event?.Date:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Ubicación:</strong> {ticket.Event?.Location}</p>
                            <p><strong>Asiento:</strong> {ticket.SeatNumber}</p>
                            <p><strong>Tipo de entrada:</strong> {ticket.Type}</p>
                        </div>
                        <div class='qr-container'>
                            <img src='data:image/png;base64,{qrCodeBase64}' 
                                 style='max-width: 200px; margin: 20px auto;'
                                 alt='Código QR de entrada' />
                            <p style='color: #666;'>Presenta este código QR en la entrada del evento</p>
                        </div>
                        <div style='text-align: center; color: #999; font-size: 12px;'>
                            <p>Este ticket es válido hasta: {ticket.ValidUntil:dd/MM/yyyy HH:mm}</p>
                            <p>ID del ticket: {ticket.Uuid}</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";
    }
}
