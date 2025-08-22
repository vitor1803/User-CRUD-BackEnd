using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace UserCRUD.Models
{
    public class Usuario
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [Required]
        public string Name { get; private set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; private set; } = string.Empty;

        [Required]
        public string SenhaHash { get; private set; } = string.Empty;

        [Required]
        public DateTime DataDeCadastro { get; private set; }

        public bool Ativo { get; private set; } = true;

        protected Usuario() { }

        public Usuario(string name, string email, string senha)
        {
            Name = name;
            Email = email;
            DefinirSenha(senha);
            DataDeCadastro = DateTime.UtcNow;
        }

        // Método para alterar o nome
        public void ChangeName(string nome)
        {
            Name = nome;
        }

        // Método para alterar o email
        public void ChangeEmail(string email)
        {
            Email = email;
        }

        // Método para definir senha com hash
        public void DefinirSenha(string senha)
        {
            SenhaHash = GerarHash(senha);
        }

        // Método para validar senha
        public bool VerificarSenha(string senha)
        {
            return SenhaHash == GerarHash(senha);
        }

        // Criptografia simples via SHA256
        private static string GerarHash(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(bytes);
        }

        // Soft delete
        public void Desativar() => Ativo = false;
    }
}
