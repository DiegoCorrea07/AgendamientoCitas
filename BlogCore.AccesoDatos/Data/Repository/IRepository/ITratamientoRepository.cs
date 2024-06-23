using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface ITratamientoRepository : IRepository<Tratamiento>
    {
        void Update(Tratamiento tratamiento);
        IEnumerable<SelectListItem> GetListaTratamientos();
    }
}
