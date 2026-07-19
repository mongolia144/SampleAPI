using SampleApi.Models;

namespace SampleApi.Interfaces.AuthInterfaces;

public interface IAuthService
{
    User? ValidateUser(string email, string password);
    string GenerateJwtToken(User user);
}
