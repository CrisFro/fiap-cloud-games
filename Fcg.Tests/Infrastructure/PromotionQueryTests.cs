using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Mocks;
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
    [Trait("Domain-infrastructure", "Promotion Queries")]
    public class PromotionQueryTests
    {
        [Fact]
        public async Task GetAllPromotionsAsync_ReturnsAllPromotions()
        {
            // Arrange
            var promotions = new List<Promotion>
            {
                new Promotion { Id = Guid.NewGuid(), Title = "Promo 1", Description = "Desc 1", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5) },
                new Promotion { Id = Guid.NewGuid(), Title = "Promo 2", Description = "Desc 2", DiscountPercent = 20, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(10) }
            };

            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var query = new PromotionQuery(mockContext.Object);

            // Act
            var result = await query.GetAllPromotionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Title == "Promo 1");
            Assert.Contains(result, p => p.Title == "Promo 2");
        }

        [Fact]
        public async Task GetAllPromotionsAsync_ReturnsEmptyList_WhenNoPromotionsExist()
        {
            // Arrange
            var promotions = new List<Promotion>();

            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var query = new PromotionQuery(mockContext.Object);

            // Act
            var result = await query.GetAllPromotionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdPromotionAsync_ReturnsPromotion_WhenPromotionExists()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var promotions = new List<Promotion>
            {
                new Promotion { Id = promotionId, Title = "Unique Promo", Description = "Unique Desc", DiscountPercent = 15, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(2) },
                new Promotion { Id = Guid.NewGuid(), Title = "Another Promo", Description = "Another Desc", DiscountPercent = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(1) }
            };

            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var query = new PromotionQuery(mockContext.Object);

            // Act
            var result = await query.GetByIdPromotionAsync(promotionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(promotionId, result.Id);
            Assert.Equal("Unique Promo", result.Title);
        }

        [Fact]
        public async Task GetByIdPromotionAsync_ReturnsNull_WhenPromotionDoesNotExist()
        {
            // Arrange
            var promotions = new List<Promotion>
            {
                new Promotion { Id = Guid.NewGuid(), Title = "Promo 1", Description = "Desc 1", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5) }
            };
            var nonExistentPromotionId = Guid.NewGuid();

            var mockSet = promotions.BuildMockDbSet();
            var mockContext = new Mock<FcgDbContext>();
            mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

            var query = new PromotionQuery(mockContext.Object);

            // Act
            var result = await query.GetByIdPromotionAsync(nonExistentPromotionId);

            // Assert
            Assert.Null(result);
        }
    }
}