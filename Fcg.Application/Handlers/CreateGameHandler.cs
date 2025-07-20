using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using MediatR;

namespace Fcg.Application.Handlers
{
    public class CreateGameHandler : IRequestHandler<CreateGameRequest, CreateGameResponse>
    {
        private readonly IGameRepository _gameRepository;

        public CreateGameHandler(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<CreateGameResponse> Handle(CreateGameRequest request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameByTitleAsync(request.Title);

            if (game != null)
            {
                return new CreateGameResponse
                {
                    Success = false,
                    Message = "Cadastro de jogo já existente"
                };
            }

            game = new Game(request.Title, request.Description, request.Genre, request.Price);

            await _gameRepository.CreateGameAsync(game);

            return new CreateGameResponse
            {
                Success = true,
                Message = "Usuário registrado com sucesso.",
                GameId = game.Id
            };
        }
    }
}
