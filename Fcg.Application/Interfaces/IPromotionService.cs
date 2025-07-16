using Fcg.Application.DTOs;

namespace Fcg.Application.Interfaces
{
    public interface IPromotionService
    {
        Task<CreatePromotionResponse> CreatePromotionAsync(CreatePromotionRequest request);
        Task<List<PromotionResponse>> GetAllPromotionsAsync();
    }
}
