using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo 'Email' é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; }

    public string FotoPerfil { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
    public string RePassword { get; set; }
}
