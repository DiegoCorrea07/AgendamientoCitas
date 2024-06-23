using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class HorarioMedicoRepository : Repository<HorarioMedico>, IHorarioMedicoRepository
    {
        private readonly ApplicationDbContext _db;

        public HorarioMedicoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaMedicos()
        {
            return _db.Medicos.Select(m => new SelectListItem()
            {
                Text = m.Nombre,
                Value = m.Id.ToString()
            }).ToList();
        }

        public IEnumerable<HorarioMedico> GetHorariosByMedico(int medicoId)
        {
            return _db.HorariosMedicos.Where(h => h.MedicoId == medicoId).ToList();
        }

        public void Update(HorarioMedico horarioMedico)
        {
            var objDesdeDb = _db.HorariosMedicos.FirstOrDefault(s => s.Id == horarioMedico.Id);
            if (objDesdeDb != null)
            {
                objDesdeDb.DiaSemana = horarioMedico.DiaSemana;
                objDesdeDb.HoraInicio = horarioMedico.HoraInicio;
                objDesdeDb.HoraFin = horarioMedico.HoraFin;

                _db.SaveChanges();
            }
        }
    }
}
