using System.ComponentModel.DataAnnotations;

namespace PlanifiqueAPI.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Nome { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        //[MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        //public string Password { get; set; }

        //[Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        //public string RePassword { get; set; }
        public string FotoPerfil { get; set; }
    }
}
