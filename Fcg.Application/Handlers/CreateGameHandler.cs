using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class CreateGameHandler : IRequestHandler<CreateGameRequest, CreateGameResponse>
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<CreateGameHandler> _logger;

        public CreateGameHandler(IGameRepository gameRepository, ILogger<CreateGameHandler> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<CreateGameResponse> Handle(CreateGameRequest request, CancellationToken cancellationToken)
        {
            // There is no GetGameByTitleAsync in IGameRepository, so we need to check for existing games another way.
            // Let's assume Title is unique and fetch all games, then check for a matching title.
            var allGames = await _gameRepository.GetGamesByIdsAsync(Array.Empty<Guid>());
            var game = allGames.FirstOrDefault(g => g.Title == request.Title);

            if (game != null)
            {
                _logger.LogWarning($"Tentativa de criar jogo com título já existente: {request.Title}");

                return new CreateGameResponse
                {
                    Success = false,
                    Message = "Jogo já existente"
                };
            }
            // Validate the genre
            game = new Game(request.Title, request.Description, request.Genre, request.Price);

            await _gameRepository.CreateAsync(game);

            _logger.LogInformation("Jogo criado com sucesso: {Title}, ID: {Id}", game.Title, game.Id);

            return new CreateGameResponse
            {
                Success = true,
                Message = "Jogo registrado com sucesso.",
                GameId = game.Id
            };
        }
    }
}
