using FluentValidation;
using Delhivery.Application.DTOs.Address;

namespace Delhivery.Application.Validators;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.ContactName).NotEmpty();
        RuleFor(x => x.MobileNumber).NotEmpty().Matches(@"^\+?\d{10,15}$");
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.Pincode).NotEmpty().Matches(@"^\d{6}$");
    }
}
