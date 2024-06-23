using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository
{
    internal class PacienteRepository : Repository<Paciente>, IPacienteRepository
    {
        private readonly ApplicationDbContext _db;

        public PacienteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IQueryable<Paciente> AsQueryable()
        {
            return _db.Set<Paciente>().AsQueryable();
        }

        public void Update(Paciente paciente)
        {
            var objDesdeDb = _db.Pacientes.FirstOrDefault(s => s.Id == paciente.Id);

            if (objDesdeDb != null)
            {
                objDesdeDb.Nombre = paciente.Nombre;
                objDesdeDb.FechaNacimiento = paciente.FechaNacimiento;
                objDesdeDb.Genero = paciente.Genero;
                objDesdeDb.Direccion = paciente.Direccion;
                objDesdeDb.Celular = paciente.Celular;
                objDesdeDb.Email = paciente.Email;
                objDesdeDb.UrlImagen = paciente.UrlImagen;
                objDesdeDb.HistorialMedico = paciente.HistorialMedico;

                _db.SaveChanges(); // Guardar los cambios en la base de datos
            }
        }



    }
}
