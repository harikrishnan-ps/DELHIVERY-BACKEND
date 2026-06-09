using Razorpay.Api;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Delhivery.Application.Interfaces;

namespace Delhivery.Infrastructure.Services;

public class RazorpayPaymentService : IPaymentService
{
    private readonly string _key;
    private readonly string _secret;

    public RazorpayPaymentService(IConfiguration configuration)
    {
        _key = configuration["Razorpay:Key"] ?? throw new ArgumentNullException("Razorpay Key missing");
        _secret = configuration["Razorpay:Secret"] ?? throw new ArgumentNullException("Razorpay Secret missing");
    }

    public Task<string> CreateOrderAsync(decimal amount, string receiptId)
    {
        var client = new RazorpayClient(_key, _secret);
        
        // Amount is in paisa
        var amountInPaisa = (int)(amount * 100);

        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", amountInPaisa },
            { "currency", "INR" },
            { "receipt", receiptId }
        };

        var order = client.Order.Create(options);
        return Task.FromResult(order["id"].ToString());
    }

    public bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
    {
        var payload = $"{orderId}|{paymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

        return computedSignature == signature;
    }
}
