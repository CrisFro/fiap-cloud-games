using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class CreatePromotionHandler : IRequestHandler<CreatePromotionRequest, CreatePromotionResponse>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<CreatePromotionHandler> _logger;

        public CreatePromotionHandler(IPromotionRepository promotionRepository, ILogger<CreatePromotionHandler> logger)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
        }

        public async Task<CreatePromotionResponse> Handle(CreatePromotionRequest request, CancellationToken cancellationToken)
        {
            var promotion = await _promotionRepository.GetPromotionByTitleAsync(request.Title);

            if (promotion != null)
            {
                _logger.LogInformation("Promoção criada com sucesso: {Title}, ID: {Id}", promotion.Title, promotion.Id);

                return new CreatePromotionResponse
                {
                    Success = false,
                    Message = "Promoção de jogo já existente"
                };
            }

            promotion = new Promotion(request.Title, request.Description, request.DiscountPercent, request.StartDate, request.EndDate);

            await _promotionRepository.CreatePromotionAsync(promotion);

            return new CreatePromotionResponse
            {
                Success = true,
                Message = "Promoção registrada com sucesso.",
                PromotionId = promotion.Id
            };
        }
    }
}
