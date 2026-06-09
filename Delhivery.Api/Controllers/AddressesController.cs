using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delhivery.Application.DTOs.Address;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;

namespace Delhivery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IRepository<Address> _addressRepository;

    public AddressesController(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = GetUserId();
        var addresses = await _addressRepository.FindAsync(a => a.UserId == userId);
        return Ok(addresses.Select(a => new AddressResponse
        {
            Id = a.Id,
            ContactName = a.ContactName,
            MobileNumber = a.MobileNumber,
            Email = a.Email,
            Flat = a.Flat,
            Area = a.Area,
            City = a.City,
            State = a.State,
            Pincode = a.Pincode,
            Tag = a.Tag
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress(AddressRequest request)
    {
        var address = new Address
        {
            UserId = GetUserId(),
            ContactName = request.ContactName,
            MobileNumber = request.MobileNumber,
            Email = request.Email,
            Flat = request.Flat,
            Area = request.Area,
            City = request.City,
            State = request.State,
            Pincode = request.Pincode,
            Tag = request.Tag
        };

        await _addressRepository.AddAsync(address);
        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, address);
    }
}
