using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Delhivery.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;

namespace Delhivery.Infrastructure.Services;

public class BrevoEmailService : IEmailService
{
    private readonly ILogger<BrevoEmailService> _logger;
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new HttpClient();

    public BrevoEmailService(IConfiguration configuration, ILogger<BrevoEmailService> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        try
        {
            var senderEmail = _configuration["Brevo:SenderEmail"] ?? "no-reply@delhiveryclone.com";
            var senderName = _configuration["Brevo:SenderName"] ?? "Delhivery Clone";
            var apiKey = _configuration["Brevo:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Brevo API Key is missing. Cannot send OTP.");
                return;
            }

            var payload = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = toEmail } },
                subject = "Your OTP for Delhivery Clone",
                htmlContent = $"<html><body><p>Your OTP is: <strong>{otp}</strong></p><p>It is valid for 5 minutes.</p></body></html>"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("api-key", apiKey);
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Brevo API rejected the OTP email request. Status: {Status}, Error: {Error}", response.StatusCode, error);
            }
            else
            {
                _logger.LogInformation("Successfully sent OTP email to {Email}", toEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
        }
    }

    public async Task SendOrderConfirmationAsync(string toEmail, string orderId)
    {
         try
        {
            var senderEmail = _configuration["Brevo:SenderEmail"] ?? "no-reply@delhiveryclone.com";
            var senderName = _configuration["Brevo:SenderName"] ?? "Delhivery Clone";
            var apiKey = _configuration["Brevo:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Brevo API Key is missing. Cannot send Order Confirmation.");
                return;
            }

            var payload = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = toEmail } },
                subject = "Order Confirmation - Delhivery Clone",
                htmlContent = $"<html><body><p>Your order <strong>{orderId}</strong> has been confirmed.</p></body></html>"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("api-key", apiKey);
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Brevo API rejected the Order Confirmation email. Status: {Status}, Error: {Error}", response.StatusCode, error);
            }
            else
            {
                _logger.LogInformation("Successfully sent Order Confirmation email to {Email}", toEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation email to {Email}", toEmail);
        }
    }
}
