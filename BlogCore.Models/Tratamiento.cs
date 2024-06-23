using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.Models
{
    public class Tratamiento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Las instrucciones son obligatorias")]
        public string Instrucciones { get; set; }

        [Required(ErrorMessage = "La dosificación es obligatoria")]
        public string Dosificacion { get; set; }

        [Required(ErrorMessage = "La frecuencia es obligatoria")]
        public string Frecuencia { get; set; }

        public string EfectosSecundarios { get; set; }

        [Required]
        public int HistorialMedicoId { get; set; }

        [ForeignKey("HistorialMedicoId")]
        public HistorialMedico HistorialMedico { get; set; }
    }
}
