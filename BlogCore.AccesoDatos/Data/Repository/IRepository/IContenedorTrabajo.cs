using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        //Aquí se deben de ir agregando los diferentes repositorios
        ICitaRepository Cita { get; }
        IPacienteRepository Paciente { get; }
        IMedicoRepository Medico { get; }
        IUsuarioRepository Usuario { get; }
        IEspecialidadRepository Especialidad { get; }
        IHistorialMedicoRepository HistorialMedico { get; }
        ITratamientoRepository Tratamiento { get; }
        ISliderRepository Slider { get; }
        IHorarioMedicoRepository HorarioMedico { get; }

        void Save();

    }
}
