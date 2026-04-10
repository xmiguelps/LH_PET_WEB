using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LH_PET_WEB.Models
{
    [Table("tb_usuario")]
    public class Usuario
    {
        [Key]
        [Column("pk_usuario")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatorio")]
        [MaxLength(255)]
        [Column("nm_usuario")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Email é obrigatorio.")]
        [EmailAddress(ErrorMessage = "E-mail invalido")]
        [MaxLength(150)]
        [Column("nm_email")]
        public string Email { get; set; } = string.Empty;
    }
}