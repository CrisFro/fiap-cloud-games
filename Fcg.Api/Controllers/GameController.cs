using Fcg.Api.Helpers;
using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Fcg.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;
        private readonly IConfiguration _configuration;

        public GameController(IGameService gameService, ILogger<GameController> logger, IConfiguration configuration)
        {
            _gameService = gameService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _gameService.RegisterGameAsync(request);

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar jogo.");
                return StatusCode(500, new { message = "Erro interno ao registrar o jogo." });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }
    }
}
