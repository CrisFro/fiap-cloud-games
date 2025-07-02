using Fcg.Application.DTOs;

namespace Fcg.Application.Interfaces
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
    }
}
