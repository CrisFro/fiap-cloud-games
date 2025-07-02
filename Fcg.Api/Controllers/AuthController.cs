using Microsoft.AspNetCore.Mvc;
using Fcg.Application.Interfaces;
using Fcg.Application.DTOs;

namespace Fcg.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _userService.RegisterUserAsync(request);

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário.");
                return StatusCode(500, new { message = "Erro interno ao registrar o usuário." });
            }
        }
    }
}
