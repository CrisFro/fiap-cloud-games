using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Fcg.Application.Validations
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
                return false;

            // Mínimo 8 caracteres, ao menos 1 letra, 1 número e 1 caractere especial
            var regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            return regex.IsMatch(password);
        }

        public override string FormatErrorMessage(string name)
        {
            return "A senha deve ter no mínimo 8 caracteres, incluindo letras, números e caracteres especiais.";
        }
    }
}
