using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.Models
{
    public class Medico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public int EspecialidadId { get; set; }

        [ForeignKey("EspecialidadId")]
        public Especialidad Especialidad { get; set; }

        public ICollection<Cita> Citas { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        public string UrlImagen { get; set; }

        [NotMapped] 
        public IEnumerable<SelectListItem> ListaEspecialidades { get; set; }

        public virtual ICollection<HorarioMedico> Horarios { get; set; }

        public Medico()
        {
            Horarios = new List<HorarioMedico>();
        }
    }
}
