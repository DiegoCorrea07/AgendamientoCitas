using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class HistorialMedicoController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HistorialMedicoController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var historialesMedicos = _contenedorTrabajo.HistorialMedico.GetAll(includeProperties: "Paciente");
            return View(historialesMedicos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PacienteId = new SelectList(_contenedorTrabajo.Paciente.GetAll(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HistorialMedico historialMedico)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.HistorialMedico.Add(historialMedico);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PacienteId = new SelectList(_contenedorTrabajo.Paciente.GetAll(), "Id", "Nombre", historialMedico.PacienteId);
            return View(historialMedico);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var historialMedico = _contenedorTrabajo.HistorialMedico.Get(id);
            if (historialMedico == null)
            {
                return NotFound();
            }

            ViewBag.PacienteId = new SelectList(_contenedorTrabajo.Paciente.GetAll(), "Id", "Nombre", historialMedico.PacienteId);
            return View(historialMedico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HistorialMedico historialMedico)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.HistorialMedico.Update(historialMedico);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PacienteId = new SelectList(_contenedorTrabajo.Paciente.GetAll(), "Id", "Nombre", historialMedico.PacienteId);
            return View(historialMedico);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var historialMedico = _contenedorTrabajo.HistorialMedico.Get(id);
            if (historialMedico == null)
            {
                return NotFound();
            }

            return View(historialMedico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var historialMedico = _contenedorTrabajo.HistorialMedico.Get(id);
            if (historialMedico == null)
            {
                return NotFound();
            }

            _contenedorTrabajo.HistorialMedico.Remove(historialMedico);
            _contenedorTrabajo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
