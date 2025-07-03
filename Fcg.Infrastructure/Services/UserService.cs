using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Fcg.Domain.Entities;

namespace Fcg.Infrastructure.Services;

public class UserService : IUserService
{
    private static readonly List<User> _users = new();

    public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
    {
        var exists = _users.Any(u => u.Email == request.Email);

        if (exists)
        {
            return new RegisterUserResponse
            {
                Success = false,
                Message = "E-mail já cadastrado."
            };
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.Password
        };

        _users.Add(user);

        return new RegisterUserResponse
        {
            Success = true,
            Message = "Usuário registrado com sucesso.",
            UserId = user.Id
        };
    }
}
