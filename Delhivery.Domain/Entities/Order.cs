using Delhivery.Domain.Enums;

namespace Delhivery.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PickupAddressId { get; set; }
    public Address PickupAddress { get; set; } = null!;

    public Guid DeliveryAddressId { get; set; }
    public Address DeliveryAddress { get; set; } = null!;

    public string PackagingType { get; set; } = string.Empty;
    public string WeightCategory { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public DateTime ScheduledPickupDate { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? RazorpayOrderId { get; set; }
    public decimal Amount { get; set; }
}
