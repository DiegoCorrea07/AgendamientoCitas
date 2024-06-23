using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class EspecialidadesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EspecialidadesController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var especialidades = _contenedorTrabajo.Especialidad.GetAll();
            return View(especialidades);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Especialidad especialidad)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Especialidad.Add(especialidad);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(especialidad);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Especialidad especialidad = _contenedorTrabajo.Especialidad.Get(id);
            if (especialidad == null)
            {
                return NotFound();
            }

            return View(especialidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Especialidad especialidad)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Especialidad.Update(especialidad);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(especialidad);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Especialidad.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var especialidadDesdeBd = _contenedorTrabajo.Especialidad.Get(id);

            if (especialidadDesdeBd == null)
            {
                return Json(new { success = false, message = "Error borrando Especialidad" });
            }

            _contenedorTrabajo.Especialidad.Remove(especialidadDesdeBd);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Especialidad Borrada Correctamente" });
        }
        #endregion
    }
}
