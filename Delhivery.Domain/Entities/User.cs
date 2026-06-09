using Delhivery.Domain.Enums;

namespace Delhivery.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
