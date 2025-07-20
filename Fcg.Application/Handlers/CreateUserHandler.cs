using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using MediatR;

namespace Fcg.Application.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public CreateUserHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailUserAsync(request.Email);

            if (user != null)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "E-mail já cadastrado."
                };
            }

            user = new User(request.Name, request.Email);

            user.SetPassword(_passwordHasherService.Hash(request.Password));

            await _userRepository.CreateUserAsync(user);

            return new CreateUserResponse
            {
                Success = true,
                Message = "Usuário registrado com sucesso.",
                UserId = user.Id
            };
        }
    }
}
