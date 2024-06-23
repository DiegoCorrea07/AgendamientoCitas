using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface ICitaRepository : IRepository<Cita>
    {
        void Update(Cita cita);
        IEnumerable<SelectListItem> GetListaMedicos();
        IEnumerable<SelectListItem> GetListaCitas();
    }
}
