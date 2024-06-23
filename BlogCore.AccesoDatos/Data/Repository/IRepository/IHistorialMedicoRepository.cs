using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IHistorialMedicoRepository : IRepository<HistorialMedico>
    {
        void Update(HistorialMedico historialMedico);
        IEnumerable<SelectListItem> GetListaHistorialesMedicos();
    }
}
