using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.Models
{
    public class Especialidad
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio")]
        public string Nombre { get; set; }

        public ICollection<Medico> Medicos { get; set; }
    }
}
