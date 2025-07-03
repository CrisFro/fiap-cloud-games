using Fcg.Application.Validations;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Application.DTOs
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; } = null!;
    }
}
