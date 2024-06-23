using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.Models.ViewModels
{
    public class TratamientoVM
    {
        public Tratamiento Tratamiento { get; set; }

        public IEnumerable<SelectListItem> ListaHistorialesMedicos { get; set; }
    }
}
