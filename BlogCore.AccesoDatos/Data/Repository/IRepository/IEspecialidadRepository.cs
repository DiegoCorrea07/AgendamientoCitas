using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IEspecialidadRepository : IRepository<Especialidad>
    {
        void Update(Especialidad especialidad);
        IEnumerable<SelectListItem> GetListaEspecialidades();
    }
}
