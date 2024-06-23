using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class EspecialidadRepository : Repository<Especialidad>, IEspecialidadRepository
    {
        private readonly ApplicationDbContext _db;

        public EspecialidadRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaEspecialidades()
        {
            return _db.Especialidades.Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString()
            });
        }

        public void Update(Especialidad especialidad)
        {
            var objDesdeDb = _db.Especialidades.FirstOrDefault(s => s.Id == especialidad.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Nombre = especialidad.Nombre;
                _db.SaveChanges();
            }
        }
    }
}
