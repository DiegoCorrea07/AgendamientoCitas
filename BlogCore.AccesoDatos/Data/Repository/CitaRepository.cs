using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class CitaRepository : Repository<Cita>, ICitaRepository
    {
        private readonly ApplicationDbContext _db;

        public CitaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaMedicos()
        {
            return _db.Medicos.Select(m => new SelectListItem()
            {
                Text = m.Nombre,
                Value = m.Id.ToString()
            });
        }

        public IEnumerable<SelectListItem> GetListaCitas()
        {
            var citas = _db.Citas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Fecha:dd/MM/yyyy} - {c.Paciente.Nombre}"
            }).ToList();

            return citas;
        }


        public void Update(Cita cita)
        {
            var objDesdeDb = _db.Citas.FirstOrDefault(s => s.Id == cita.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Fecha = cita.Fecha;
                objDesdeDb.PacienteId = cita.PacienteId;
                objDesdeDb.MedicoId = cita.MedicoId;
                objDesdeDb.Observaciones = cita.Observaciones;

                _db.SaveChanges();
            }
        }
    }
}
