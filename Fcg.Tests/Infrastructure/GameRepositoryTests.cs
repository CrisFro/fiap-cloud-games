using Fcg.Domain.Entities;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests
{
    public class GameRepositoryTests : BaseRepositoryTests
    {
        private readonly GameRepository _gameRepository;

        public GameRepositoryTests()
        {
            _gameRepository = new GameRepository(_context);
        }

        [Fact]
        public async Task CreateGameAsync_ShouldAddGameToDatabase()
        {
            // Arrange
            var game = EntityFakers.GameFaker.Generate();

            // Act
            var gameId = await _gameRepository.CreateGameAsync(game);

            // Assert
            gameId.Should().NotBeEmpty();
            var savedGameEntity = await _context.Games.FindAsync(gameId);
            savedGameEntity.Should().NotBeNull();
            savedGameEntity.Title.Should().Be(game.Title);
            savedGameEntity.Description.Should().Be(game.Description);
            savedGameEntity.Genre.Should().Be((int)game.Genre);
            savedGameEntity.Price.Should().Be(game.Price);
        }

        [Fact]
        public async Task GetGameByTitleAsync_ShouldReturnGame_WhenGameExists()
        {
            // Arrange
            var game = EntityFakers.GameFaker.Generate();
            await _gameRepository.CreateGameAsync(game);

            // Act
            var retrievedGame = await _gameRepository.GetGameByTitleAsync(game.Title);

            // Assert
            retrievedGame.Should().NotBeNull();
            retrievedGame.Id.Should().Be(game.Id);
            retrievedGame.Title.Should().Be(game.Title);
        }

        [Fact]
        public async Task GetGameByTitleAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Arrange
            var nonExistentTitle = "Non Existent Game";

            // Act
            var retrievedGame = await _gameRepository.GetGameByTitleAsync(nonExistentTitle);

            // Assert
            retrievedGame.Should().BeNull();
        }

        [Fact]
        public async Task GetGamesByIdsAsync_ShouldReturnGames_WhenGamesExist()
        {
            // Arrange
            var games = EntityFakers.GameFaker.Generate(3); // Gerar 3 jogos
            foreach (var game in games)
            {
                await _gameRepository.CreateGameAsync(game);
            }
            var gameIds = games.Select(g => g.Id).ToList();

            // Act
            var retrievedGames = await _gameRepository.GetGamesByIdsAsync(gameIds);

            // Assert
            retrievedGames.Should().NotBeNull();
            retrievedGames.Should().HaveCount(3);
            retrievedGames.Select(g => g.Id).Should().BeEquivalentTo(gameIds);
        }

        [Fact]
        public async Task GetGamesByIdsAsync_ShouldReturnEmptyList_WhenNoGamesExistForIds()
        {
            // Arrange
            var nonExistentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var retrievedGames = await _gameRepository.GetGamesByIdsAsync(nonExistentIds);

            // Assert
            retrievedGames.Should().NotBeNull();
            retrievedGames.Should().BeEmpty();
        }
    }
}