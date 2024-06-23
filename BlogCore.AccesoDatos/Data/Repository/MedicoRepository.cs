using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlogCore.AccesoDatos.Data.Repository
{
    internal class MedicoRepository : Repository<Medico>, IMedicoRepository
    {
        private readonly ApplicationDbContext _db;

        public MedicoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaEspecialidades()
        {
            return _db.Especialidades.Select(e => new SelectListItem()
            {
                Text = e.Nombre,
                Value = e.Id.ToString()
            });
        }

        public IQueryable<Medico> AsQueryable()
        {
            return _db.Medicos.AsQueryable();
        }

        public Medico Get(int id, string includeProperties = null)
        {
            IQueryable<Medico> query = _db.Medicos;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault(m => m.Id == id);
        }

        public Medico GetFirstOrDefault(Expression<Func<Medico, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<Medico> query = _db.Medicos;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public void Update(Medico medico)
        {
            var objDesdeDb = _db.Medicos.FirstOrDefault(s => s.Id == medico.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.Nombre = medico.Nombre;
                objDesdeDb.Especialidad = medico.Especialidad;
                objDesdeDb.UrlImagen = medico.UrlImagen;

                _db.SaveChanges();
            }
        }
    }
}
