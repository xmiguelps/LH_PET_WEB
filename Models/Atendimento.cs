using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LH_PET_WEB.Models
{
    [Table("tb_atendimento")]
    public class Atendimento
    {
        [Key]
        [Column("pk_atendimento")]
        public int Id { get; set; }

        [Required]
        [Column("fk_agendamento")]
        public int AgendamentoId { get; set; }

        [Required(ErrorMessage = "O Prontuario é obrigatorio.")]
        [Column("ds_prontuario")]
        public string Prontuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Diagnostico é obrigatorio.")]
        [Column("ds_diagnostico")]
        public string Diagnostico { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Prescrição é obrigatoria.")]
        [Column("ds_prescricao")]
        public string Prescricao { get; set; } = string.Empty;

        [ForeignKey("AgendamentoId")]
        public Agendamento? Agendamento { get; set; }
    }
}