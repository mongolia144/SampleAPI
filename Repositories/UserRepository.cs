using SampleApi.Models;
using SampleApi.Interfaces.UserInterfaces;

namespace SampleApi.Repositories;

public class UserRepository : IUserRepository
{
    // Temporary in-memory user list
    private readonly List<User> _users = new()
    {
        new User
        {
            Email = "test@example.com",
            Password = "password123"
        }
    };

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }
}
