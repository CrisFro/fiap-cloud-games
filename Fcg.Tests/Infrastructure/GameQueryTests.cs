using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Mocks; // Importa as extensões do Mock
using Fcg.Domain.Queries.Responses;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fcg.Infrastructure.Tests.Queries
{
    [Trait("Domain-infrastructure", "Game Queries")]
    public class GameQueryTests
    {
        //[Fact]
        //public async Task GetAllGamesAsync_ReturnsAllGames()
        //{
        //    // Arrange
        //    var games = new List<Game>
        //    {
        //        new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "Desc 1", Genre = "Action", Price = 59.90m, CreatedAt = DateTime.UtcNow },
        //        new Game { Id = Guid.NewGuid(), Title = "Game 2", Description = "Desc 2", Genre = "RPG", Price = 99.90m, CreatedAt = DateTime.UtcNow }
        //    };

        //    var mockSet = games.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Games).Returns(mockSet.Object);

        //    var query = new GameQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetAllGamesAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Count());
        //    Assert.Contains(result, g => g.Title == "Game 1");
        //    Assert.Contains(result, g => g.Title == "Game 2");
        //}

        //[Fact]
        //public async Task GetAllGamesAsync_ReturnsEmptyList_WhenNoGamesExist()
        //{
        //    // Arrange
        //    var games = new List<Game>(); // Lista vazia

        //    var mockSet = games.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Games).Returns(mockSet.Object);

        //    var query = new GameQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetAllGamesAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Empty(result);
        //}

        //[Fact]
        //public async Task GetByIdGameAsync_ReturnsGame_WhenGameExists()
        //{
        //    // Arrange
        //    var gameId = Guid.NewGuid();
        //    var games = new List<Game>
        //    {
        //        new Game { Id = gameId, Title = "Unique Game", Description = "Unique Desc", Genre = "Adventure", Price = 79.90m, CreatedAt = DateTime.UtcNow },
        //        new Game { Id = Guid.NewGuid(), Title = "Another Game", Description = "Another Desc", Genre = "Strategy", Price = 49.90m, CreatedAt = DateTime.UtcNow }
        //    };

        //    var mockSet = games.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Games).Returns(mockSet.Object);

        //    var query = new GameQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetByIdGameAsync(gameId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(gameId, result.Id);
        //    Assert.Equal("Unique Game", result.Title);
        //}

        //[Fact]
        //public async Task GetByIdGameAsync_ReturnsNull_WhenGameDoesNotExist()
        //{
        //    // Arrange
        //    var games = new List<Game>
        //    {
        //        new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "Desc 1", Genre = "Action", Price = 59.90m, CreatedAt = DateTime.UtcNow }
        //    };
        //    var nonExistentGameId = Guid.NewGuid();

        //    var mockSet = games.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Games).Returns(mockSet.Object);

        //    var query = new GameQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetByIdGameAsync(nonExistentGameId);

        //    // Assert
        //    Assert.Null(result);
        //}
    }
}