using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delhivery.Application.DTOs.Auth;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;
using Delhivery.Infrastructure.Persistence;

namespace Delhivery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly DelhiveryDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IServiceProvider _serviceProvider;

    public OtpController(DelhiveryDbContext dbContext, IEmailService emailService, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _serviceProvider = serviceProvider;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { title = "Email is required" });

        // Generate a 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();

        var otpLog = new OTPLog
        {
            Email = request.Email,
            OTP = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.OTPLogs.Add(otpLog);
        await _dbContext.SaveChangesAsync();

        // Send OTP via Brevo in the background using a safe DI scope
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scopedEmailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await scopedEmailService.SendOtpAsync(request.Email, otp);
            }
            catch
            {
                // Background exception swallowed
            }
        });

        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
            return BadRequest(new { title = "Email and OTP are required" });

        var otpLog = await _dbContext.OTPLogs
            .Where(x => x.Email == request.Email && x.OTP == request.Otp && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpLog == null)
            return BadRequest(new { title = "Invalid OTP" });

        if (otpLog.ExpiresAt < DateTime.UtcNow)
            return BadRequest(new { title = "OTP has expired" });

        // Mark as used
        otpLog.IsUsed = true;
        otpLog.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "OTP verified successfully" });
    }
}
