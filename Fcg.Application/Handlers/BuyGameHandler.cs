using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic; 
using System.Linq; 
using System; 

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

            // Verifica se todos os jogos solicitados foram encontrados
            if (games == null || games.Count() != request.GamesIds.Count())
            {
                _logger.LogWarning("Alguns jogos não foram encontrados!");

                return new BuyGameResponse
                {
                    Success = false,
                    Message = "Alguns jogos não foram encontrados!"
                };
            }

            var gamesAlreadyOwned = new List<string>();
            foreach (var game in games)
            {
                if (user.Library.Any(ug => ug.Game.Id == game.Id))
                {
                    gamesAlreadyOwned.Add(game.Title);
                }
            }

            if (gamesAlreadyOwned.Any())
            {
                var message = $"O usuário já possui os seguintes jogos na biblioteca: {string.Join(", ", gamesAlreadyOwned)}";
                _logger.LogWarning(message);
                return new BuyGameResponse
                {
                    Success = false,
                    Message = message
                };
            }

            foreach (var game in games)
            {
                user.AddGameToLibrary(game);
            }


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