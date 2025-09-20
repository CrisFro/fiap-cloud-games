using Fcg.Domain.Entities;
using Fcg.Infrastructure.Repositories;
using Fcg.Tests.Infrastructure;
using FluentAssertions;

namespace Fcg.Infrastructure.Tests
{
    [Trait("Domain-infrastructure", "Game Repository")]
    public class GameRepositoryTests : BaseRepositoryTests
    {
        private readonly GameRepository _gameRepository;

        public GameRepositoryTests()
        {
            // Supondo que GameRepository exista e siga o mesmo padrão de UserRepository
            _gameRepository = new GameRepository(_context);
        }

        [Fact]
        public void CreateAsync_ShouldThrowArgumentException_WhenCreatingGameWithNegativePrice()
        {
            // Arrange
            Action act = () => new Game("Test Game", "Test Description", GenreEnum.Acao, -9.99m);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}