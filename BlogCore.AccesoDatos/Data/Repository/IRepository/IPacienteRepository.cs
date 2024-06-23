using BlogCore.Models;
using System.Linq;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IPacienteRepository : IRepository<Paciente>
    {
        void Update(Paciente paciente);

        // Método para el buscador
        IQueryable<Paciente> AsQueryable();
    }
}
