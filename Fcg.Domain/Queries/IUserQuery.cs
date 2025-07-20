using Fcg.Domain.Queries.Responses;

namespace Fcg.Domain.Queries
{
    public interface IUserQuery
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> GetByIdUserAsync(Guid userId);
    }
}
