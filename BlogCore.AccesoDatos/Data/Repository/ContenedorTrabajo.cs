using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;

        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Cita = new CitaRepository(_db);
            Paciente = new PacienteRepository(_db);
            Medico = new MedicoRepository(_db);
            Usuario = new UsuarioRepository(_db);
            Especialidad = new EspecialidadRepository(_db);
            HistorialMedico = new HistorialMedicoRepository(_db);
            Tratamiento = new TratamientoRepository(_db);
            Slider = new SliderRepository(_db);
            HorarioMedico = new HorarioMedicoRepository(_db);
        }

        public ICitaRepository Cita { get; private set; }
        public IPacienteRepository Paciente { get; private set; }
        public IMedicoRepository Medico { get; private set; }
        public IUsuarioRepository Usuario { get; private set; }
        public IEspecialidadRepository Especialidad { get; private set; }
        public IHistorialMedicoRepository HistorialMedico { get; private set; }
        public ITratamientoRepository Tratamiento { get; private set; }
        public ISliderRepository Slider { get; private set; }
        public IHorarioMedicoRepository HorarioMedico { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
