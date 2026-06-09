namespace Delhivery.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string ContactName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Flat { get; set; }
    public string? Area { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    
    public string Tag { get; set; } = "Home"; // Home, Office, etc.
}
