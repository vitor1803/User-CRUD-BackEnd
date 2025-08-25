using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public record UsuarioCadastroDTO(
        [property: Required, MinLength(3), Description("Nome completo do usuário")] string name,

        [property: Required, EmailAddress, Description("E-mail do usuário (único no sistema)")] string email,

        [property: Required, MinLength(6), Description("Senha do usuário (mínimo 6 caracteres)")] string password
    );
}