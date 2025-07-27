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
    [Trait("Domain-infrastructure", "Promotion Repository")]
    public class PromotionRepositoryTests
    {
        [Fact]
        public async Task CreatePromotionAsync_AddsPromotionToDatabase()
        {
            // Arrange
            var promotions = new List<Tables.Promotion>();
            var mockSet = promotions.BuildMockDbSet();
            mockSet.Setup(m => m.Add(It.IsAny<Tables.Promotion>())).Callback<Tables.Promotion>(promotions.Add);

            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new PromotionRepository(mockContext.Object);
            var newPromotion = new Fcg.Domain.Entities.Promotion(Guid.NewGuid(), "New Test Promo", "Description of new test promo", 25, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(10));

            // Act
            var createdId = await repository.CreatePromotionAsync(newPromotion);

            // Assert
            Assert.NotEqual(Guid.Empty, createdId);
            mockSet.Verify(m => m.Add(It.IsAny<Tables.Promotion>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Single(promotions);
            Assert.Equal(newPromotion.Title, promotions.First().Title);
        }

        [Fact]
        public async Task GetPromotionByTitleAsync_ReturnsPromotion_WhenTitleExists()
        {
            // Arrange
            var existingPromotion = new Tables.Promotion { Id = Guid.NewGuid(), Title = "Existing Promo", Description = "Desc", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5) };
            var promotions = new List<Tables.Promotion> { existingPromotion };

            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var repository = new PromotionRepository(mockContext.Object);

            // Act
            var result = await repository.GetPromotionByTitleAsync("Existing Promo");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingPromotion.Id, result.Id);
            Assert.Equal(existingPromotion.Title, result.Title);
        }

        [Fact]
        public async Task GetPromotionByTitleAsync_ReturnsNull_WhenTitleDoesNotExist()
        {
            // Arrange
            var promotions = new List<Tables.Promotion>();
            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var repository = new PromotionRepository(mockContext.Object);

            // Act
            var result = await repository.GetPromotionByTitleAsync("Non Existent Promo");

            // Assert
            Assert.Null(result);
        }
    }
}