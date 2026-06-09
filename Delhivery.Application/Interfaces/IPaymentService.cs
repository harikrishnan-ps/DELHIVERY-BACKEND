namespace Delhivery.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateOrderAsync(decimal amount, string receiptId);
    bool VerifyPaymentSignature(string orderId, string paymentId, string signature);
}
