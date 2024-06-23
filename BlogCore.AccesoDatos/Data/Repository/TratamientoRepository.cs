using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class TratamientoRepository : Repository<Tratamiento>, ITratamientoRepository
    {
        private readonly ApplicationDbContext _db;

        public TratamientoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaTratamientos()
        {
            return _db.Tratamientos.Select(i => new SelectListItem()
            {
                Text = i.Descripcion,
                Value = i.Id.ToString()
            });
        }

        public void Update(Tratamiento tratamiento)
        {
            var objDesdeDb = _db.Tratamientos.FirstOrDefault(s => s.Id == tratamiento.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Descripcion = tratamiento.Descripcion;
                objDesdeDb.Instrucciones = tratamiento.Instrucciones;
                objDesdeDb.Dosificacion = tratamiento.Dosificacion;
                objDesdeDb.Frecuencia = tratamiento.Frecuencia;
                objDesdeDb.EfectosSecundarios = tratamiento.EfectosSecundarios;
                _db.SaveChanges();
            }
        }
    }
}
