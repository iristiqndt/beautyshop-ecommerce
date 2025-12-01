using ECommerce.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ECommerce.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation($"Preparing to send email to {to} with subject '{subject}'");
            
            var fromAddress = _configuration["Email:FromAddress"];
            var fromName = _configuration["Email:FromName"];
            var smtpServer = _configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var useOAuth = bool.Parse(_configuration["Email:UseOAuth"] ?? "false");
            
            _logger.LogInformation($"SMTP Config: Server={smtpServer}, Port={smtpPort}, From={fromAddress}, Username={username}, UseOAuth={useOAuth}");
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName ?? "ECommerce", fromAddress ?? "noreply@ecommerce.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 10000; // 10 seconds timeout
            
            _logger.LogInformation($"Connecting to SMTP server {smtpServer}:{smtpPort}");
            
            // Kiểm tra nếu dùng port 465 (SSL) hoặc 587 (TLS)
            if (smtpPort == 465)
            {
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.SslOnConnect);
            }
            else
            {
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            }
            
            // Chỉ authenticate nếu có username và password
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _logger.LogInformation($"Authenticating with username {username}");
                await client.AuthenticateAsync(username, password);
            }
            else
            {
                _logger.LogInformation("No authentication required (using anonymous SMTP)");
            }
            
            _logger.LogInformation("Sending email...");
            await client.SendAsync(message);
            
            await client.DisconnectAsync(true);
            _logger.LogInformation($"Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}: {ex.Message}");
            throw;
        }
    }

    public async Task SendRegistrationConfirmationAsync(string email, string fullName)
    {
        var subject = "Welcome to BeautyShop!";
        var body = $@"
            <h2>Welcome {fullName}!</h2>
            <p>Thank you for registering with BeautyShop.</p>
            <p>You can now start shopping for your favorite cosmetics!</p>
            <br/>
            <p>Best regards,</p>
            <p>BeautyShop Team</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var resetUrl = $"{_configuration["App:FrontendUrl"]}/reset-password?token={resetToken}";
        var subject = "Reset Your Password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Click the link below to reset your password:</p>
            <p><a href='{resetUrl}'>Reset Password</a></p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, please ignore this email.</p>
            <br/>
            <p>Best regards,</p>
            <p>BeautyShop Team</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendOrderConfirmationAsync(string email, string orderNumber, decimal totalAmount)
    {
        var subject = $"Order Confirmation - {orderNumber}";
        var body = $@"
            <h2>Order Confirmed!</h2>
            <p>Thank you for your order.</p>
            <p><strong>Order Number:</strong> {orderNumber}</p>
            <p><strong>Total Amount:</strong> ${totalAmount:F2}</p>
            <p>We'll send you another email when your order ships.</p>
            <br/>
            <p>Best regards,</p>
            <p>BeautyShop Team</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordChangedAsync(string email, string fullName)
    {
        var subject = "Password Changed Successfully";
        var body = $@"
            <h2>Hello {fullName},</h2>
            <p>Your password has been changed successfully.</p>
            <p>If you did not make this change, please contact us immediately.</p>
            <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
            <br/>
            <p>Best regards,</p>
            <p>BeautyShop Team</p>
        ";

        await SendEmailAsync(email, subject, body);
    }
}
