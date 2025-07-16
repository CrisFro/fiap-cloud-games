using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionController> _logger;
        private readonly IConfiguration _configuration;

        public PromotionController(IPromotionService promotionService, ILogger<PromotionController> logger, IConfiguration configuration)
        {
            _promotionService = promotionService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatePromotionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _promotionService.CreatePromotionAsync(request);

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar a promoção.");
                return StatusCode(500, new { message = "Erro interno ao criar a promoção." });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAllPromotionsAsync();
            return Ok(promotions);
        }
    }
}
