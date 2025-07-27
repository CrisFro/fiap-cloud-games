using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Repositories;
using Fcg.Domain.Entities;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Mocks; // Importa as extensões do Mock
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fcg.Infrastructure.Tests.Repositories
{
    [Trait("Domain-infrastructure", "Game Repository")]
    public class GameRepositoryTests
    {
        [Fact]
        public async Task CreateGameAsync_AddsGameToDatabase()
        {
            // Arrange
            var games = new List<Tables.Game>(); // Lista vazia para simular o banco
            var mockSet = games.BuildMockDbSet();

            mockSet.Setup(m => m.Add(It.IsAny<Tables.Game>())).Callback<Tables.Game>(games.Add); // Simula o Add

            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Games).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Simula SaveChangesAsync

            var repository = new GameRepository(mockContext.Object);
            var newGame = new Fcg.Domain.Entities.Game(Guid.NewGuid(), "New Test Game", "Description of new test game", "Strategy", 39.99m, DateTime.UtcNow);

            // Act
            var createdId = await repository.CreateGameAsync(newGame);

            // Assert
            Assert.NotEqual(Guid.Empty, createdId);
            mockSet.Verify(m => m.Add(It.IsAny<Tables.Game>()), Times.Once()); // Verifica se Add foi chamado
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once()); // Verifica se SaveChangesAsync foi chamado
            Assert.Single(games); // Verifica se o jogo foi adicionado à lista mocada
            Assert.Equal(newGame.Title, games.First().Title);
        }

        [Fact]
        public async Task GetGameByTitleAsync_ReturnsGame_WhenTitleExists()
        {
            // Arrange
            var existingGame = new Tables.Game { Id = Guid.NewGuid(), Title = "Existing Game", Description = "Desc", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow };
            var games = new List<Tables.Game> { existingGame };

            var mockSet = games.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            var repository = new GameRepository(mockContext.Object);

            // Act
            var result = await repository.GetGameByTitleAsync("Existing Game");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingGame.Id, result.Id);
            Assert.Equal(existingGame.Title, result.Title);
        }


        [Fact]
        public async Task GetGameByTitleAsync_ReturnsNull_WhenTitleDoesNotExist()
        {
            // Arrange
            var games = new List<Tables.Game>(); // Lista vazia

            var mockSet = games.BuildMockDbSet();

            // Crie uma instância vazia de DbContextOptions para o construtor
            var options = new DbContextOptions<FcgDbContext>();
            var mockContext = new Mock<FcgDbContext>(options); // PASSE AS OPÇÕES AQUI

            mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            var repository = new GameRepository(mockContext.Object);

            // Act
            var result = await repository.GetGameByTitleAsync("Non Existent Game");

            // Assert
            Assert.Null(result);
        }







        [Fact]
        public async Task GetGamesByIdsAsync_ReturnsCorrectGames_WhenIdsExist()
        {
            // Arrange
            var game1 = new Tables.Game { Id = Guid.NewGuid(), Title = "Game A", Description = "Desc A", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow };
            var game2 = new Tables.Game { Id = Guid.NewGuid(), Title = "Game B", Description = "Desc B", Genre = "RPG", Price = 20m, CreatedAt = DateTime.UtcNow };
            var game3 = new Tables.Game { Id = Guid.NewGuid(), Title = "Game C", Description = "Desc C", Genre = "Puzzle", Price = 30m, CreatedAt = DateTime.UtcNow };
            var games = new List<Tables.Game> { game1, game2, game3 };

            var mockSet = games.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            var repository = new GameRepository(mockContext.Object);
            var idsToRetrieve = new List<Guid> { game1.Id, game3.Id };

            // Act
            var result = await repository.GetGamesByIdsAsync(idsToRetrieve);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, g => g.Id == game1.Id);
            Assert.Contains(result, g => g.Id == game3.Id);
            Assert.DoesNotContain(result, g => g.Id == game2.Id);
        }

        [Fact]
        public async Task GetGamesByIdsAsync_ReturnsEmptyList_WhenNoMatchingIds()
        {
            // Arrange
            var games = new List<Tables.Game> { new Tables.Game { Id = Guid.NewGuid(), Title = "Game X", Description = "Desc X", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow } };
            var mockSet = games.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            var repository = new GameRepository(mockContext.Object);
            var nonExistentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var result = await repository.GetGamesByIdsAsync(nonExistentIds);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGamesByIdsAsync_ReturnsEmptyList_WhenInputIdsAreEmpty()
        {
            // Arrange
            var games = new List<Tables.Game> { new Tables.Game { Id = Guid.NewGuid(), Title = "Game X", Description = "Desc X", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow } };
            var mockSet = games.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            var repository = new GameRepository(mockContext.Object);
            var emptyIds = new List<Guid>();

            // Act
            var result = await repository.GetGamesByIdsAsync(emptyIds);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}