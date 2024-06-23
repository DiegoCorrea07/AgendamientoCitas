using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models.ViewModels;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;


namespace TuProyecto.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class CitasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CitasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // Método para validar la disponibilidad del médico en la fecha y hora seleccionadas
        private bool ValidarDisponibilidadMedico(int medicoId, DateTime fecha, TimeSpan hora)
        {
            // Obtener el día de la semana de la fecha seleccionada
            var diaSemanaSeleccionado = fecha.DayOfWeek;

            // Buscar los horarios del médico para el día seleccionado
            var horariosMedico = _contenedorTrabajo.HorarioMedico.GetAll(h => h.MedicoId == medicoId);

            // Verificar si hay algún horario que coincida con el día de la semana y la hora seleccionadas
            foreach (var horario in horariosMedico)
            {
                if (HorarioMedico.DiaSemanaToDayOfWeek[horario.DiaSemana] == diaSemanaSeleccionado &&
                    hora >= horario.HoraInicio && hora < horario.HoraFin)
                {
                    return true; // El médico está disponible en esta hora
                }
            }

            return false; // El médico no está disponible en esta hora
        }

        // Método para enviar un recordatorio de cita al paciente
        private void EnviarRecordatorioCita(Cita cita)
        {
            // Implementar la lógica real para enviar el recordatorio al paciente (correo electrónico, SMS, etc.)
            var paciente = _contenedorTrabajo.Paciente.Get(cita.PacienteId);
            if (paciente != null)
            {
                string mensaje = $"Recordatorio de cita médica el {cita.Fecha.ToShortDateString()} a las {cita.Fecha.ToString(@"hh\:mm")}.";
                // Lógica para enviar el mensaje por correo electrónico, SMS, etc.
                // Ejemplo simulado:
                // EmailService.SendEmail(paciente.CorreoElectronico, "Recordatorio de cita médica", mensaje);
            }
        }

        // GET: Admin/Citas/Index
        [HttpGet]
        public IActionResult Index()
        {
            var citas = _contenedorTrabajo.Cita.GetAll(includeProperties: "Medico,Paciente");
            return View(citas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CitaVM
            {
                ListaMedicos = _contenedorTrabajo.Medico.GetAll()
                                .Select(m => new SelectListItem
                                {
                                    Value = m.Id.ToString(),
                                    Text = m.Nombre
                                })
                                .ToList(),
                ListaPacientes = _contenedorTrabajo.Paciente.GetAll()
                                .Select(p => new SelectListItem
                                {
                                    Value = p.Id.ToString(),
                                    Text = p.Nombre
                                })
                                .ToList(),
                CitasDisponibles = new List<Cita>() // Inicializamos una lista vacía para las citas disponibles
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetCitasDisponibles(int medicoId, DateTime fecha, CitaVM model)
        {
            DateTime fechaSeleccionada = model.Cita.Fecha;

            // Obtener el día de la semana correspondiente a la fecha seleccionada
            DayOfWeek diaSemanaSeleccionado = fechaSeleccionada.DayOfWeek;

            // Convertir DayOfWeek a un valor compatible con tu modelo de datos si es necesario
            int diaSemana = (int)diaSemanaSeleccionado;

            // Luego puedes usar 'diaSemana' en tu consulta LINQ para comparar con 'DiaSemana' en tu modelo
            var horarios = _contenedorTrabajo.HorarioMedico.GetAll(
                h => h.MedicoId == medicoId && (int)h.DiaSemana == diaSemana
            ).ToList();

            // Obtener todas las citas del médico para la fecha seleccionada
            var citasDelMedico = _contenedorTrabajo.Cita.GetAll(
                c => c.MedicoId == medicoId && c.Fecha.Date == fecha.Date
            ).ToList();

            // Filtrar los horarios disponibles según las citas existentes
            var citasDisponibles = new List<Cita>();
            foreach (var horario in horarios)
            {
                var horaInicio = horario.HoraInicio;
                var horaFin = horario.HoraFin;

                // Verificar si la hora de inicio del horario está disponible
                bool disponible = true;
                foreach (var cita in citasDelMedico)
                {
                    if (cita.Hora >= horaInicio && cita.Hora < horaFin)
                    {
                        disponible = false;
                        break;
                    }
                }

                // Si el horario está disponible, agregarlo a la lista de citas disponibles
                if (disponible)
                {
                    citasDisponibles.Add(new Cita
                    {
                        Fecha = fecha,
                        Hora = horaInicio,
                        MedicoId = medicoId
                    });
                }
            }

            return Json(new { data = citasDisponibles });
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cita = _contenedorTrabajo.Cita.Get(id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cita cita)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Cita.Update(cita);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(cita);
        }

        // Método para obtener la disponibilidad de horas del médico mediante AJAX
        [HttpGet]
        public IActionResult GetDisponibilidad(int medicoId, DateTime fecha)
        {
            // Obtener el día de la semana de la fecha seleccionada
            var diaSemanaSeleccionado = fecha.DayOfWeek;

            // Obtener los horarios del médico para el día de la semana seleccionado
            var horarios = _contenedorTrabajo.HorarioMedico.GetAll(
                h => h.MedicoId == medicoId && HorarioMedico.DiaSemanaToDayOfWeek[h.DiaSemana] == diaSemanaSeleccionado
            );

            // Obtener todas las citas del médico para la fecha seleccionada
            var citasDelMedico = _contenedorTrabajo.Cita.GetAll(
                c => c.MedicoId == medicoId && c.Fecha.Date == fecha.Date
            );

            // Filtrar horarios disponibles eliminando los horarios en los que ya hay citas
            var disponibilidad = horarios.Select(h => new
            {
                HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                HoraFin = h.HoraFin.ToString(@"hh\:mm")
            }).Where(h =>
                !citasDelMedico.Any(c =>
                    c.Hora >= TimeSpan.Parse(h.HoraInicio) && c.Hora < TimeSpan.Parse(h.HoraFin)
                )
            );

            return Json(new { data = disponibilidad });
        }


        #region Llamadas a la API

        // Método para obtener todas las citas en formato JSON
        [HttpGet]
        public IActionResult GetAll()
        {
            var citas = _contenedorTrabajo.Cita.GetAll(includeProperties: "Paciente,Medico");
            var citasVM = citas.Select(c => new
            {
                c.Id,
                c.Fecha,
                c.Hora,
                PacienteNombre = c.Paciente != null ? c.Paciente.Nombre : "No asignado",
                MedicoNombre = c.Medico != null ? c.Medico.Nombre : "No asignado",
                c.Observaciones
            });

            return Json(new { data = citasVM });
        }

        // Método para eliminar una cita
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var citaDesdeBd = _contenedorTrabajo.Cita.Get(id);
            if (citaDesdeBd == null)
            {
                return Json(new { success = false, message = "Error borrando cita" });
            }

            _contenedorTrabajo.Cita.Remove(citaDesdeBd);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Cita borrada correctamente" });
        }

        #endregion
    }
}
