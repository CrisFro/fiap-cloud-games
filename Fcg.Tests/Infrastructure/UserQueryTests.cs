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
    [Trait("Domain-infrastructure", "User Queries")]
    public class UserQueryTests
    {
        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com", Role = "Player" },
                new User { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@example.com", Role = "Admin" }
            };

            var mockSet = users.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Name == "User 1");
            Assert.Contains(result, u => u.Name == "User 2");
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();

            var mockSet = users.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdUserAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = userId, Name = "Unique User", Email = "unique@example.com", Role = "Player" },
                new User { Id = Guid.NewGuid(), Name = "Another User", Email = "another@example.com", Role = "Admin" }
            };

            var mockSet = users.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetByIdUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Unique User", result.Name);
        }

        [Fact]
        public async Task GetByIdUserAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com", Role = "Player" }
            };
            var nonExistentUserId = Guid.NewGuid();

            var mockSet = users.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetByIdUserAsync(nonExistentUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLibraryByUserAsync_ReturnsGamesInUserLibrary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var game1Id = Guid.NewGuid();
            var game2Id = Guid.NewGuid();

            var users = new List<User>
            {
                new User { Id = userId, Name = "Library User", Email = "library@example.com", Role = "Player" }
            };
            var games = new List<Game>
            {
                new Game { Id = game1Id, Title = "Game A", Description = "Desc A", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow },
                new Game { Id = game2Id, Title = "Game B", Description = "Desc B", Genre = "Puzzle", Price = 20m, CreatedAt = DateTime.UtcNow }
            };
            var userGamings = new List<UserGaming>
            {
                new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = game1Id, Game = games.First(g => g.Id == game1Id), PurchasedDate = DateTime.UtcNow },
                new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = game2Id, Game = games.First(g => g.Id == game2Id), PurchasedDate = DateTime.UtcNow }
            };

            var mockUserSet = users.BuildMockDbSet();
            var mockGameSet = games.BuildMockDbSet();
            var mockUserGamingSet = userGamings.BuildMockDbSet();

            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Games).Returns(mockGameSet.Object);
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetLibraryByUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, g => g.Title == "Game A");
            Assert.Contains(result, g => g.Title == "Game B");
        }

        [Fact]
        public async Task GetLibraryByUserAsync_ReturnsEmptyList_WhenUserHasNoGames()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = userId, Name = "Empty Library User", Email = "empty@example.com", Role = "Player" }
            };
            var games = new List<Game>();
            var userGamings = new List<UserGaming>(); // Vazio

            var mockUserSet = users.BuildMockDbSet();
            var mockGameSet = games.BuildMockDbSet();
            var mockUserGamingSet = userGamings.BuildMockDbSet();

            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Games).Returns(mockGameSet.Object);
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);

            var query = new UserQuery(mockContext.Object);

            // Act
            var result = await query.GetLibraryByUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLibraryByUserAsync_ReturnsEmptyList_WhenUserDoesNotExist()
        {
            // Arrange
            var users = new List<User>(); // Nenhum usuário
            var games = new List<Game> { new Game { Id = Guid.NewGuid(), Title = "Game A", Description = "Desc A", Genre = "Action", Price = 10m, CreatedAt = DateTime.UtcNow } };
            var userGamings = new List<UserGaming>();

            var mockUserSet = users.BuildMockDbSet();
            var mockGameSet = games.BuildMockDbSet();
            var mockUserGamingSet = userGamings.BuildMockDbSet();

            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Games).Returns(mockGameSet.Object);
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);

            var query = new UserQuery(mockContext.Object);

            var nonExistentUserId = Guid.NewGuid();

            // Act
            var result = await query.GetLibraryByUserAsync(nonExistentUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}