using Microsoft.AspNetCore.Identity;

namespace AvaloniaSimpleChat.Server.Services;

public interface IUserService
{
    bool VerifyOrCreateUser(string username, string password);
}

public sealed class UserService : IUserService
{
    private readonly Dictionary<string, string> _users = new();
    private readonly Lock _usersLock = new();
    private readonly PasswordHasher<string> _passwordHasher = new();

    public bool VerifyOrCreateUser(string username, string password)
    {
        lock (_usersLock)
        {
            if (_users.TryGetValue(username, out var storedHash))
                return _passwordHasher.VerifyHashedPassword(username, storedHash, password) ==
                       PasswordVerificationResult.Success;

            _users[username] = _passwordHasher.HashPassword(username, password);
            return true;
        }
    }
}

