using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Fcg.Domain.Entities;

namespace Fcg.Infrastructure.Services;

public class PromotionService : IPromotionService
{
    private static readonly List<Promotion> _Promotion = new();

    public async Task<CreatePromotionResponse> CreatePromotionAsync(CreatePromotionRequest request)
    {
        var exists = _Promotion.Any(u => u.Title == request.Title);

        if (exists)
        {
            return new CreatePromotionResponse
            {
                Success = false,
                Message = "Promoção já existente"
            };
        }

        var Promotion = new Promotion
        {
            PromotionId = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DiscountPercent = request.DiscountPercent,
        };

        _Promotion.Add(Promotion);

        return new CreatePromotionResponse
        {
            Success = true,
            Message = "Promoção criada com sucesso.",
            GameId = Promotion.PromotionId
        };
    }
}
