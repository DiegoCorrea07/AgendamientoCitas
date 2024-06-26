﻿using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models.ViewModels;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
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

        // Método para enviar un recordatorio de cita al paciente (ejemplo simulado)
        private void EnviarRecordatorioCita(Cita cita)
        {
            var paciente = _contenedorTrabajo.Paciente.Get(cita.PacienteId);
            if (paciente != null)
            {
                string mensaje = $"Recordatorio de cita médica el {cita.Fecha.ToShortDateString()} a las {cita.Hora.ToString(@"hh\:mm")}.";
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

            // Retornar los resultados en formato JSON
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
