using Fcg.Application.Responses;
using MediatR;

namespace Fcg.Application.Requests
{
    public class UpdateRoleRequest : IRequest<UpdateRoleResponse>
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; } = null!;
    }
}
