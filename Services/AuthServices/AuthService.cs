using SampleApi.Models;
using SampleApi.Interfaces.AuthInterfaces;
using SampleApi.Interfaces.UserInterfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleApi.Services.AuthServices;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository,  IConfiguration config)
    {
        _userRepository = userRepository;
         _config = config;
    }

    public User? ValidateUser(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);

        if (user is null)
            return null;

        if (user.Password != password)
            return null;

        return user;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSection = _config.GetSection("Jwt");

        var keyString = jwtSection.GetValue<string>("Key");
        if (keyString is null)
            throw new Exception("JWT Key is missing in configuration");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpiresInMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
