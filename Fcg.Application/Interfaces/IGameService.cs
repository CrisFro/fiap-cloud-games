using Fcg.Application.DTOs;

namespace Fcg.Application.Interfaces
{
    public interface IGameService
    {
        Task<RegisterGameResponse> RegisterGameAsync(RegisterGameRequest request);
    }
}
