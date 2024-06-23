using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.Models.ViewModels
{
    public class PacienteVM
    {
        public Paciente Paciente { get; set; }
        //public IEnumerable<SelectListItem> ListaCitas { get; set; }
        public ICollection<HistorialMedico> HistorialesMedicos { get; set; }
    }
}
