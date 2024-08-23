using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required]
    public string Nome { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string FotoPerfil { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
    public string RePassword { get; set; }
}
