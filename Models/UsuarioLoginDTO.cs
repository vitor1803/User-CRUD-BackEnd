using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserCRUD.Models
{
    public record UsuarioLoginDTO(
        [property: Required, EmailAddress, Description("E-mail do usuário para login")] string email,
        [property: Required, Description("Senha do usuário para login")] string password);
}