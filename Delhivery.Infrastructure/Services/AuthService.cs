using BCrypt.Net;
using Delhivery.Application.DTOs.Auth;
using Delhivery.Application.Interfaces;
using Delhivery.Domain.Entities;

namespace Delhivery.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IRepository<User> userRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.FindAsync(u => u.Email == request.Email);
        if (existingUser.Any())
            throw new Exception("User already exists with this email.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = request.Role
        };

        await _userRepository.AddAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var tokens = await _refreshTokenRepository.FindAsync(rt => rt.Token == token);
        var refreshToken = tokens.FirstOrDefault();

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        refreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var token = _jwtTokenGenerator.GenerateToken(user);
        var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshTokenString,
            UserId = user.Id,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }
}
