using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class TratamientosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public TratamientosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tratamientos = _contenedorTrabajo.Tratamiento.GetAll(includeProperties: "HistorialMedico");
            return View(tratamientos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.HistorialMedicoId = new SelectList(_contenedorTrabajo.HistorialMedico.GetAll(), "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tratamiento tratamiento)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Tratamiento.Add(tratamiento);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.HistorialMedicoId = new SelectList(_contenedorTrabajo.HistorialMedico.GetAll(), "Id", "Id", tratamiento.HistorialMedicoId);
            return View(tratamiento);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tratamiento = _contenedorTrabajo.Tratamiento.Get(id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            ViewBag.HistorialMedicoId = new SelectList(_contenedorTrabajo.HistorialMedico.GetAll(), "Id", "Id", tratamiento.HistorialMedicoId);
            return View(tratamiento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tratamiento tratamiento)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Tratamiento.Update(tratamiento);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.HistorialMedicoId = new SelectList(_contenedorTrabajo.HistorialMedico.GetAll(), "Id", "Id", tratamiento.HistorialMedicoId);
            return View(tratamiento);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var tratamiento = _contenedorTrabajo.Tratamiento.Get(id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            return View(tratamiento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tratamiento = _contenedorTrabajo.Tratamiento.Get(id);
            if (tratamiento == null)
            {
                return NotFound();
            }

            _contenedorTrabajo.Tratamiento.Remove(tratamiento);
            _contenedorTrabajo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
