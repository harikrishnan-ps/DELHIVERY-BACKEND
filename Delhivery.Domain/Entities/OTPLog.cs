namespace Delhivery.Domain.Entities;

public class OTPLog : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
