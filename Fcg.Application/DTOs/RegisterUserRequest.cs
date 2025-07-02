using Fcg.Application.Validations;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Application.DTOs
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(3, ErrorMessage = "O nome deve ter pelo menos 3 caracteres.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StrongPassword]
        public string Password { get; set; } = null!;
    }
}