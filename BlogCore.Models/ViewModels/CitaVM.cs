using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.Models.ViewModels
{
    public class CitaVM
    {
        public Cita Cita { get; set; }
        public string HoraSeleccionada { get; set; }
        public List<Cita> ListaCitas { get; set; }
        public List<Cita> CitasDisponibles { get; set; }

        public IEnumerable<SelectListItem> ListaPacientes { get; set; }
        public IEnumerable<SelectListItem> ListaMedicos { get; set; }
    }
}
