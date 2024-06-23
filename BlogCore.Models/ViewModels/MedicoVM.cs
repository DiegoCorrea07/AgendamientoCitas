using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.Models.ViewModels
{
    public class MedicoVM
    {
        public Medico Medico { get; set; }
        public IEnumerable<SelectListItem> ListaEspecialidades { get; set; }
        // Otras propiedades que puedan ser necesarias en la vista
    }
}
