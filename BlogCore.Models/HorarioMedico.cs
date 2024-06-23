using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlogCore.Models.HorarioMedico;

namespace BlogCore.Models
{
    public class HorarioMedico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }

        [Required]
        public DiaSemana DiaSemana { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [CustomValidation(typeof(HorarioMedico), nameof(ValidarHoraFin))]
        public TimeSpan HoraFin { get; set; }

        public virtual Medico Medico { get; set; }

        // Validación para asegurar que HoraFin es mayor que HoraInicio
        public static ValidationResult ValidarHoraFin(TimeSpan horaFin, ValidationContext context)
        {
            var instance = context.ObjectInstance as HorarioMedico;
            if (instance != null && instance.HoraInicio >= horaFin)
            {
                return new ValidationResult("La hora de fin debe ser mayor que la hora de inicio.");
            }
            return ValidationResult.Success;
        }

        public static Dictionary<DiaSemana, DayOfWeek> DiaSemanaToDayOfWeek = new Dictionary<DiaSemana, DayOfWeek>
        {
            { DiaSemana.Lunes, DayOfWeek.Monday },
            { DiaSemana.Martes, DayOfWeek.Tuesday },
            { DiaSemana.Miércoles, DayOfWeek.Wednesday },
            { DiaSemana.Jueves, DayOfWeek.Thursday },
            { DiaSemana.Viernes, DayOfWeek.Friday },
            { DiaSemana.Sábado, DayOfWeek.Saturday },
            { DiaSemana.Domingo, DayOfWeek.Sunday }
        };

    }
    public enum DiaSemana
    {
        Lunes,
        Martes,
        Miércoles,
        Jueves,
        Viernes,
        Sábado,
        Domingo
    }
}
