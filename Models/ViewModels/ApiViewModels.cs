using System;
using System.ComponentModel.DataAnnotations;
using LH_PET_WEB.Validations;

namespace LH_PET_WEB.Models.ViewModels
{
    public class ApiLoginDTO
    {
        [Required(ErrorMessage = "O E-mail é obrigatorio.")]
        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatorio.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class ApiRegistroDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatorio.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter pelo menos 3 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatorio.")]
        [Cpf(ErrorMessage = "O CPF informado é matematicamente invalido.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatorio.")]
        [RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$", ErrorMessage = "Formato de telefone invalido.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatoria.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "A senha é muito fraca.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class ApiPetDTO
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Especie { get; set; } = string.Empty;

        [Required]
        public string Raca { get; set; } = string.Empty;

        [Required]
        public DateTime DataNascimento { get; set; }
    }

    public class ApiAgendamentoDTO
    {
        [Required]
        public int PetId { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;
    }

    public class ApiPerfilUpdateDTO
    {
        [Required]
        [MinLength(3)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$")]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}