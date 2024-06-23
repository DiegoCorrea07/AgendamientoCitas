using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class HistorialMedicoRepository : Repository<HistorialMedico>, IHistorialMedicoRepository
    {
        private readonly ApplicationDbContext _db;

        public HistorialMedicoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaHistorialesMedicos()
        {
            return _db.HistorialesMedicos.Select(i => new SelectListItem()
            {
                Text = i.Id.ToString(),
                Value = i.Id.ToString()
            });
        }

        public void Update(HistorialMedico historialMedico)
        {
            var objDesdeDb = _db.HistorialesMedicos.FirstOrDefault(s => s.Id == historialMedico.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.PacienteId = historialMedico.PacienteId;
                objDesdeDb.Paciente = historialMedico.Paciente;
                // Actualiza las propiedades necesarias
                _db.SaveChanges();
            }
        }
    }
}
