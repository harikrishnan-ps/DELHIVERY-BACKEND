using Delhivery.Domain.Entities;

namespace Delhivery.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
