using FluentValidation;
using Delhivery.Application.DTOs.Order;

namespace Delhivery.Application.Validators;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.PickupAddressId).NotEmpty();
        RuleFor(x => x.DeliveryAddressId).NotEmpty();
        RuleFor(x => x.PackagingType).NotEmpty();
        RuleFor(x => x.WeightCategory).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
        RuleFor(x => x.ScheduledPickupDate).GreaterThan(DateTime.UtcNow.AddDays(-1));
    }
}
