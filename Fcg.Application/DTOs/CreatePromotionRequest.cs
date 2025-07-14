using System.ComponentModel.DataAnnotations;

public class CreatePromotionRequest
{
    [Required]
    [Range(0.01, 1.0, ErrorMessage = "O desconto deve ser entre 0.01 e 1.0 (percentual).")]
    public decimal DiscountPercent { get; set; }

    [Required(ErrorMessage = "A data de início da promoção é obrigatória.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "A data de termino da promoção é obrigatória.")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "A descrição da promoção é obrigatória.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "O título da promoção é obrigatório.")]
    public string Title { get; set; } = null!;
}