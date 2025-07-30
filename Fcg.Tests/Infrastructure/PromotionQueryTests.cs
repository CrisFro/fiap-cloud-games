using Fcg.Domain.Queries.Responses;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Mocks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests.Queries
{
    [Trait("Domain-infrastructure", "Promotion Queries")]
    public class PromotionQueryTests
    {
        //[Fact]
        //public async Task GetAllPromotionsAsync_ReturnsAllPromotions()
        //{
        //    // Arrange
        //    var promotions = new List<Promotion>
        //    {
        //        new Promotion { Id = Guid.NewGuid(), Title = "Promo 1", Description = "Desc 1", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5) },
        //        new Promotion { Id = Guid.NewGuid(), Title = "Promo 2", Description = "Desc 2", DiscountPercent = 20, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(10) }
        //    };

        //    var mockSet = promotions.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

        //    var query = new PromotionQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetAllPromotionsAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Count());
        //    Assert.Contains(result, p => p.Title == "Promo 1");
        //    Assert.Contains(result, p => p.Title == "Promo 2");
        //}

        //[Fact]
        //public async Task GetAllPromotionsAsync_ReturnsEmptyList_WhenNoPromotionsExist()
        //{
        //    // Arrange
        //    var promotions = new List<Promotion>();

        //    var mockSet = promotions.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

        //    var query = new PromotionQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetAllPromotionsAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Empty(result);
        //}

        //[Fact]
        //public async Task GetByIdPromotionAsync_ReturnsPromotion_WhenPromotionExists()
        //{
        //    // Arrange
        //    var promotionId = Guid.NewGuid();
        //    var promotionTitle = "Test Promotion Title";
        //    var existingPromotion = new Domain.Entities.Promotion(
        //        promotionId,
        //        promotionTitle,
        //        "Description",
        //        10m,
        //        DateTime.UtcNow.AddDays(-10),
        //        DateTime.UtcNow.AddDays(10)
        //    );

        //    // Crie um DbSet mockável
        //    var promotions = new List<Tables.Promotion>
        //    {
        //        new Tables.Promotion // Entidade de infraestrutura/tabela
        //        {
        //            Id = existingPromotion.Id,
        //            Title = existingPromotion.Title,
        //            Description = existingPromotion.Description,
        //            DiscountPercent = existingPromotion.DiscountPercent,
        //            StartDate = existingPromotion.StartDate,
        //            EndDate = existingPromotion.EndDate
        //        }
        //    }.AsQueryable();

        //    var mockSet = new Mock<DbSet<Tables.Promotion>>();
        //    mockSet.As<IQueryable<Tables.Promotion>>().Setup(m => m.Provider).Returns(promotions.Provider);
        //    mockSet.As<IQueryable<Tables.Promotion>>().Setup(m => m.Expression).Returns(promotions.Expression);
        //    mockSet.As<IQueryable<Tables.Promotion>>().Setup(m => m.ElementType).Returns(promotions.ElementType);
        //    mockSet.As<IQueryable<Tables.Promotion>>().Setup(m => m.GetEnumerator()).Returns(promotions.GetEnumerator());

        //    // Mock do FcgDbContext, configurando a propriedade Promotions para retornar o DbSet mockado
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

        //    // Instancie o repositório com o DbContext mockado
        //    var repository = new PromotionRepository(mockContext.Object);

        //    // Act
        //    var result = await repository.GetPromotionByTitleAsync(promotionTitle); // Ajuste o método de busca conforme sua necessidade

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Id.Should().Be(promotionId);
        //    result.Title.Should().Be(promotionTitle);
        //    mockContext.Verify(m => m.Promotions, Times.Once()); // Opcional: verifica que a propriedade foi acessada
        //}


        //[Fact]
        //public async Task GetByIdPromotionAsync_ReturnsNull_WhenPromotionDoesNotExist()
        //{
        //    // Arrange
        //    var promotions = new List<Promotion>
        //    {
        //        new Promotion { Id = Guid.NewGuid(), Title = "Promo 1", Description = "Desc 1", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5) }
        //    };
        //    var nonExistentPromotionId = Guid.NewGuid();

        //    var mockSet = promotions.BuildMockDbSet();
        //    var mockContext = new Mock<FcgDbContext>();
        //    mockContext.Setup(c => c.Promotions).Returns(mockSet.Object);

        //    var query = new PromotionQuery(mockContext.Object);

        //    // Act
        //    var result = await query.GetByIdPromotionAsync(nonExistentPromotionId);

        //    // Assert
        //    Assert.Null(result);
        //}
    }
}