using Delhivery.Domain.Enums;
using Delhivery.Application.DTOs.Address;

namespace Delhivery.Application.DTOs.Order;

public class OrderRequest
{
    public Guid PickupAddressId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public string PackagingType { get; set; } = string.Empty;
    public string WeightCategory { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime ScheduledPickupDate { get; set; }
}

public class OrderResponse : OrderRequest
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? RazorpayOrderId { get; set; }
    public decimal Amount { get; set; }
    public AddressResponse PickupAddress { get; set; } = null!;
    public AddressResponse DeliveryAddress { get; set; } = null!;
}

public class PaymentVerificationRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}
