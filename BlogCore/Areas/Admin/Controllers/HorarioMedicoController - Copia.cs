using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class HorariosMedicosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HorariosMedicosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var horariosMedicos = _contenedorTrabajo.HorarioMedico.GetAll(includeProperties: "Medico");
            return View(horariosMedicos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var horarioMedico = new HorarioMedico();

            // Obtener y cargar la lista de médicos desde tu repositorio o base de datos
            var medicos = _contenedorTrabajo.Medico.GetAll().Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            ViewBag.ListaMedicos = new SelectList(medicos, "Value", "Text");

            return View(horarioMedico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HorarioMedico horarioMedico)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.HorarioMedico.Add(horarioMedico);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            // Recargar la lista de médicos en caso de errores de validación
            var medicos = _contenedorTrabajo.Medico.GetAll().Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            ViewBag.ListaMedicos = new SelectList(medicos, "Value", "Text");

            return View(horarioMedico);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var horarioMedico = _contenedorTrabajo.HorarioMedico.Get(id);
            if (horarioMedico == null)
            {
                return NotFound();
            }

            // Obtener y cargar la lista de médicos desde tu repositorio o base de datos
            var medicos = _contenedorTrabajo.Medico.GetAll().Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            ViewBag.ListaMedicos = new SelectList(medicos, "Value", "Text");

            return View(horarioMedico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HorarioMedico horarioMedico)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.HorarioMedico.Update(horarioMedico);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            // Recargar la lista de médicos en caso de errores de validación
            var medicos = _contenedorTrabajo.Medico.GetAll().Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre
            }).ToList();

            ViewBag.ListaMedicos = new SelectList(medicos, "Value", "Text");

            return View(horarioMedico);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var horarioMedicoDesdeBd = _contenedorTrabajo.HorarioMedico.Get(id);
            if (horarioMedicoDesdeBd == null)
            {
                return Json(new { success = false, message = "Error borrando horario médico" });
            }

            _contenedorTrabajo.HorarioMedico.Remove(horarioMedicoDesdeBd);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Horario médico borrado correctamente" });
        }
    }
}
