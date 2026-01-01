using ShoeEcommerce.Application.Common.Interfaces.Services;

namespace ShoeEcommerce.Infrastructure.Services;
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}