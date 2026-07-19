using SampleApi.Models;
using SampleApi.Interfaces.UserInterfaces;

namespace SampleApi.Interfaces.UserInterfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
}
