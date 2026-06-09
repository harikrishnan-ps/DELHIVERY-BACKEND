namespace Delhivery.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp);
    Task SendOrderConfirmationAsync(string toEmail, string orderId);
}
