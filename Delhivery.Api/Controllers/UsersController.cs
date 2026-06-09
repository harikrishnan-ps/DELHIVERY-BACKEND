using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delhivery.Application.DTOs.Profile;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;

namespace Delhivery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepository;

    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userRepository.GetByIdAsync(GetUserId());
        if (user == null) return NotFound("User not found");

        return Ok(new ProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString()
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(GetUserId());
        if (user == null) return NotFound("User not found");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        
        await _userRepository.UpdateAsync(user);

        return Ok(new { Message = "Profile updated successfully" });
    }
}
