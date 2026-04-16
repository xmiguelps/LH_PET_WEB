using System.ComponentModel.DataAnnotations;

namespace LH_PET_WEB.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatorio.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatoria.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class EsqueciSenhaViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatorio.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail é invalido.")]
        public string Email { get; set; } = string.Empty;
    }

    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatorio.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatorio")]
        [EmailAddress(ErrorMessage = "Formato de e-mail invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Perfil é obrigatorio.")]
        public string Perfil { get; set; } = "Funcionario";
    }

    public class RedefinirSenhaViewModel
    {
        [Required(ErrorMessage = "A nova senha é obrigatoria.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "A senha deve ter no minimo 8 caracteres, contendo pelo menos 1 letra maiuscula, 1 minuscula, 1 minuscula, 1 numero e 1 caractere especial.")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}