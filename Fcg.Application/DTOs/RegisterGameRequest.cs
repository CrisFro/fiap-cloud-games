using Fcg.Application.Validations;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Application.DTOs
{
    public class RegisterGameRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(3, ErrorMessage = "O nome deve ter pelo menos 3 caracteres.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MinLength(50, ErrorMessage = "A descrição deve ter pelo menos 50 caracteres.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
        [DataType(DataType.Date, ErrorMessage = "A data de lançamento deve ser uma data válida.")]
        public string CreatedAt { get; set; } = null!;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public string Genre { get; set; } = null!;

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }
    }
}