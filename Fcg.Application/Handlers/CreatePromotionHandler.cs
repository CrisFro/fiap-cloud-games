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
            try
            {
                var title = request.Title?.Trim();
                var description = request.Description?.Trim();

                if (!Enum.IsDefined(typeof(GenreEnum), request.Genre))
                {
                    _logger.LogWarning("Tentativa de criar promoção com gênero inválido: {Genre}", request.Genre);
                    return new CreatePromotionResponse
                    {
                        Success = false,
                        Message = "Gênero inválido para a promoção."
                    };
                }

                var promotion = new Promotion(
                    title!,
                    description!,
                    request.DiscountPercent,
                    request.StartDate,
                    request.EndDate,
                    request.Genre
                );

                await _promotionRepository.UpdateAsync(promotion);

                _logger.LogInformation(
                    "Promoção criada com sucesso: {Title}, ID: {Id}, Gênero: {Genre}",
                    promotion.Title,
                    promotion.Id,
                    promotion.Genre
                );

                return new CreatePromotionResponse
                {
                    Success = true,
                    Message = "Promoção registrada com sucesso.",
                    PromotionId = promotion.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar promoção: {Title}", request.Title);
                return new CreatePromotionResponse
                {
                    Success = false,
                    Message = "Falha ao criar promoção. Tente novamente."
                };
            }
        }
    }
}