using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TuProyecto.Areas.Cliente.Controllers // Asegúrate de que esté en el área "Cliente"
{
    [Authorize(Roles = CNT.Registrado)]
    [Area("Cliente")]
    public class CitasClienteController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly UserManager<IdentityUser> _userManager;

        public CitasClienteController(IContenedorTrabajo contenedorTrabajo, UserManager<IdentityUser> userManager)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _userManager = userManager;
        }

        // Método para enviar un recordatorio de cita al paciente (ejemplo simulado)
        private void EnviarRecordatorioCita(Cita cita)
        {
            var paciente = _contenedorTrabajo.Paciente.Get(cita.PacienteId);
            if (paciente != null)
            {
                string mensaje = $"Recordatorio de cita médica el {cita.Fecha.ToShortDateString()} a las {cita.Hora.ToString(@"hh\:mm")}.";
                // Lógica para enviar el mensaje por correo electrónico, SMS, etc.
               
                // EmailService.SendEmail(paciente.CorreoElectronico, "Recordatorio de cita médica", mensaje);
            }
        }


        // GET: Cliente/CitasCliente
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var citas = _contenedorTrabajo.Cita.GetAll(c => c.Paciente.UserId == User.Identity.Name, includeProperties: "Medico,Paciente");

            return View(citas);
        }

        // GET: Cliente/CitasCliente/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            var viewModel = new CitaVM
            {
                ListaMedicos = _contenedorTrabajo.Medico.GetAll()
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    })
                    .ToList(),
                ListaPacientes = _contenedorTrabajo.Paciente.GetAll(p => p.UserId == userId) // Filtrar pacientes por el cliente actual
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    })
                    .ToList(),
                CitasDisponibles = new List<Cita>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public List<Cita> GetCitasDisponibles(int medicoId, DateTime fecha, CitaVM model)
        {
            // Obtener el día de la semana correspondiente a la fecha seleccionada por el usuario
            DayOfWeek diaSemanaSeleccionado = fecha.DayOfWeek;

            // Convertir DayOfWeek a un valor compatible con tu modelo de datos si es necesario
            int diaSemana = (int)diaSemanaSeleccionado;

            // Obtener todos los horarios del médico para el día de la semana seleccionado
            var horarios = _contenedorTrabajo.HorarioMedico.GetAll(
                h => h.MedicoId == medicoId && (int)h.DiaSemana == diaSemana
            ).ToList();

            // Obtener todas las citas del médico para la fecha seleccionada
            var citasDelMedico = _contenedorTrabajo.Cita.GetAll(
                c => c.MedicoId == medicoId && c.Fecha.Date == fecha.Date
            ).ToList();

            // Crear una lista para almacenar los horarios disponibles
            var citasDisponibles = new List<Cita>();

            // Iterar sobre los horarios del médico para verificar la disponibilidad
            foreach (var horario in horarios)
            {
                var horaInicio = horario.HoraInicio;
                var horaFin = horario.HoraFin;

                // Verificar cada media hora dentro del horario del médico
                while (horaInicio < horaFin)
                {
                    // Verificar si la hora de inicio del horario está disponible
                    if (!citasDelMedico.Any(c => c.Hora == horaInicio))
                    {
                        // Si está disponible, agregarlo a la lista de citas disponibles
                        citasDisponibles.Add(new Cita
                        {
                            Fecha = fecha,  // Asignamos la fecha seleccionada por el usuario
                            Hora = horaInicio,  // Asignamos la hora de inicio del horario disponible
                            MedicoId = medicoId  // Asignamos el Id del médico
                        });
                    }

                    // Incrementar en 30 minutos para el siguiente intervalo
                    horaInicio = horaInicio.Add(TimeSpan.FromMinutes(30));
                }
            }

            return citasDisponibles;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CitaVM model)
        {
            if (ModelState.IsValid)
            {
                // Mapear los datos del ViewModel al modelo de entidad Cita
                var nuevaCita = new Cita
                {
                    Fecha = model.Cita.Fecha,
                    Hora = model.Cita.Hora,
                    PacienteId = model.Cita.PacienteId,
                    MedicoId = model.Cita.MedicoId,
                    Observaciones = model.Cita.Observaciones
                };

                // Guardar la nueva cita en el repositorio
                _contenedorTrabajo.Cita.Add(nuevaCita);
                _contenedorTrabajo.Save();

                // Ejemplo de envío de recordatorio (descomentar para usar)
                // EnviarRecordatorioCita(nuevaCita);

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores de validación, recargar la vista con el modelo y las listas actualizadas
            model.ListaMedicos = _contenedorTrabajo.Medico.GetAll()
                                    .Select(m => new SelectListItem
                                    {
                                        Value = m.Id.ToString(),
                                        Text = m.Nombre
                                    })
                                    .ToList();
            model.ListaPacientes = _contenedorTrabajo.Paciente.GetAll()
                                    .Select(p => new SelectListItem
                                    {
                                        Value = p.Id.ToString(),
                                        Text = p.Nombre
                                    })
                                    .ToList();

            return View(model);
        }

        private readonly Dictionary<DayOfWeek, int> diaSemanaMap = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Monday, 0 },
            { DayOfWeek.Tuesday, 1 },
            { DayOfWeek.Wednesday, 2 },
            { DayOfWeek.Thursday, 3 },
            { DayOfWeek.Friday, 4 },
            { DayOfWeek.Saturday, 5 },
            { DayOfWeek.Sunday, 6 }
        };

        [HttpGet]
        public IActionResult GetDisponibilidad(int medicoId, DateTime fecha)
        {
            var diaSemanaSeleccionado = diaSemanaMap[fecha.DayOfWeek];

            // Convertir DayOfWeek a un valor compatible con tu modelo de datos si es necesario
            int diaSemana = (int)diaSemanaSeleccionado;

            // Obtener todos los horarios del médico para el día de la semana seleccionado
            var horarios = _contenedorTrabajo.HorarioMedico.GetAll(
                h => h.MedicoId == medicoId && (int)h.DiaSemana == diaSemana
            ).ToList();

            // Si no hay horarios para ese día, retornar una lista vacía
            if (!horarios.Any())
            {
                return Json(new { data = new List<string>() });
            }

            // Obtener todas las citas del médico para la fecha seleccionada
            var citasDelMedico = _contenedorTrabajo.Cita.GetAll(
                c => c.MedicoId == medicoId && c.Fecha.Date == fecha.Date
            ).ToList();

            var disponibilidad = new List<string>();

            foreach (var horario in horarios)
            {
                var horaInicio = horario.HoraInicio;
                var horaFin = horario.HoraFin;

                while (horaInicio < horaFin)
                {
                    var intervalo = horaInicio.ToString(@"hh\:mm");

                    // Verificar si el intervalo está ocupado por una cita existente
                    if (!citasDelMedico.Any(c => c.Hora == horaInicio))
                    {
                        disponibilidad.Add(intervalo);
                    }

                    // Incrementar en 30 minutos
                    horaInicio = horaInicio.Add(TimeSpan.FromMinutes(30));
                }
            }

            return Json(new { data = disponibilidad });
        }
    }
}
