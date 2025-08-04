using Fcg.Domain.Entities;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests
{
    public class PromotionRepositoryTests : BaseRepositoryTests
    {
        private readonly PromotionRepository _promotionRepository;

        public PromotionRepositoryTests()
        {
            _promotionRepository = new PromotionRepository(_context);
        }

        [Fact]
        public async Task CreatePromotionAsync_ShouldAddPromotionToDatabase()
        {
            // Arrange
            var promotion = EntityFakers.PromotionFaker.Generate();

            // Act
            var promotionId = await _promotionRepository.CreatePromotionAsync(promotion);

            // Assert
            promotionId.Should().NotBeEmpty();
            var savedPromotionEntity = await _context.Promotions.FindAsync(promotionId);
            savedPromotionEntity.Should().NotBeNull();
            savedPromotionEntity.Title.Should().Be(promotion.Title);
            savedPromotionEntity.Description.Should().Be(promotion.Description);
            savedPromotionEntity.DiscountPercent.Should().Be(promotion.DiscountPercent);
            savedPromotionEntity.StartDate.Should().BeCloseTo(promotion.StartDate, TimeSpan.FromSeconds(1)); // Comparar DateTime com tolerância
            savedPromotionEntity.EndDate.Should().BeCloseTo(promotion.EndDate, TimeSpan.FromSeconds(1));     // Comparar DateTime com tolerância
        }

        [Fact]
        public async Task GetPromotionByTitleAsync_ShouldReturnPromotion_WhenPromotionExists()
        {
            // Arrange
            var promotion = EntityFakers.PromotionFaker.Generate();
            await _promotionRepository.CreatePromotionAsync(promotion);

            // Act
            var retrievedPromotion = await _promotionRepository.GetPromotionByTitleAsync(promotion.Title);

            // Assert
            retrievedPromotion.Should().NotBeNull();
            retrievedPromotion.Id.Should().Be(promotion.Id);
            retrievedPromotion.Title.Should().Be(promotion.Title);
        }

        [Fact]
        public async Task GetPromotionByTitleAsync_ShouldReturnNull_WhenPromotionDoesNotExist()
        {
            // Arrange
            var nonExistentTitle = "Non Existent Promotion";

            // Act
            var retrievedPromotion = await _promotionRepository.GetPromotionByTitleAsync(nonExistentTitle);

            // Assert
            retrievedPromotion.Should().BeNull();
        }
    }
}