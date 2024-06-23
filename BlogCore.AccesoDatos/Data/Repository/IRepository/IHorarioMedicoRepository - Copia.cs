using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IHorarioMedicoRepository : IRepository<HorarioMedico>
    {
        IEnumerable<SelectListItem> GetListaMedicos();
        IEnumerable<HorarioMedico> GetHorariosByMedico(int medicoId);
        void Update(HorarioMedico horarioMedico);
    }
}
