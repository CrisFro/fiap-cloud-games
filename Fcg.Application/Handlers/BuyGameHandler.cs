using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class BuyGameHandler : IRequestHandler<BuyGameRequest, BuyGameResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<BuyGameHandler> _logger;

        public BuyGameHandler(IUserRepository userRepository, IGameRepository gameRepository, ILogger<BuyGameHandler> logger)
        {
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<BuyGameResponse> Handle(BuyGameRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning($"Usuário com Id {request.UserId} não encontrado!");

                return new BuyGameResponse
                {
                    Success = false,
                    Message = "Usuário não encontrado!"
                };
            }

            var games = await _gameRepository.GetGamesByIdsAsync(request.GamesIds);

            if (games!.Count() != request.GamesIds.Count())
            {
                _logger.LogWarning("Alguns jogos não foram encontrados!");

                return new BuyGameResponse
                {
                    Success = false,
                    Message = "Alguns jogos não foram encontrados!"
                };
            }

            var userGames = new List<Domain.Entities.UserGaming>();

            foreach (var game in games!)
            {
                if (user.Library!.Any(ug => ug.Game.Id == game.Id))
                {
                    _logger.LogWarning($"O usuário já possui o jogo {game.Title} na biblioteca!");
                    return new BuyGameResponse
                    {
                        Success = false,
                        Message = $"O usuário já possui o jogo {game.Title} na biblioteca!"
                    };
                }

                userGames.Add(new Domain.Entities.UserGaming(game, DateTime.UtcNow));
            }

            user.UpdateGameLibrary(userGames);

            await _userRepository.UpdateUserLibraryAsync(user);

            return new BuyGameResponse
            {
                Success = true,
                Message = "Jogos comprados com sucesso!",
                GamesIds = request.GamesIds,
                UserId = user.Id
            };
        }
    }
}
