using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class MedicosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<MedicosController> _logger;

        public MedicosController(IContenedorTrabajo contenedorTrabajo,
            IWebHostEnvironment hostingEnvironment, ILogger<MedicosController> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var medicos = _contenedorTrabajo.Medico.GetAll(includeProperties: "Especialidad");
            return View(medicos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var medicoVM = new MedicoVM
            {
                Medico = new Medico(),
                ListaEspecialidades = _contenedorTrabajo.Especialidad.GetAll().Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
            };

            return View(medicoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicoVM medicoVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (archivos.Count > 0)
                {
                    // Procesar la imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subida = Path.Combine(rutaPrincipal, @"imagenes\medicos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subida, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    medicoVM.Medico.UrlImagen = @"\imagenes\medicos\" + nombreArchivo + extension;
                }

                _contenedorTrabajo.Medico.Add(medicoVM.Medico);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            medicoVM.ListaEspecialidades = _contenedorTrabajo.Especialidad.GetAll().Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nombre
            });

            return View(medicoVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var medicoVM = new MedicoVM
            {
                Medico = new Medico(),
                ListaEspecialidades = _contenedorTrabajo.Especialidad.GetAll().Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                })
            };

            if (id == null)
            {
                return NotFound();
            }

            medicoVM.Medico = _contenedorTrabajo.Medico.Get(id.Value, includeProperties: "Especialidad");

            if (medicoVM.Medico == null)
            {
                return NotFound();
            }

            return View(medicoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MedicoVM medicoVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var medicoDesdeBd = _contenedorTrabajo.Medico.Get(medicoVM.Medico.Id);

                if (archivos.Count > 0)
                {
                    // Eliminar la imagen anterior si existe
                    var rutaImagen = Path.Combine(rutaPrincipal, medicoDesdeBd.UrlImagen.TrimStart('\\'));
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    // Procesar la nueva imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subida = Path.Combine(rutaPrincipal, @"imagenes\medicos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subida, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    medicoVM.Medico.UrlImagen = @"\imagenes\medicos\" + nombreArchivo + extension;
                }
                else
                {
                    // Conservar la imagen existente si no se sube una nueva
                    medicoVM.Medico.UrlImagen = medicoDesdeBd.UrlImagen;
                }

                _contenedorTrabajo.Medico.Update(medicoVM.Medico);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, volver a cargar la lista de especialidades y mostrar la vista de edición con errores
            medicoVM.ListaEspecialidades = _contenedorTrabajo.Especialidad.GetAll().Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nombre
            });

            return View(medicoVM);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var medicos = _contenedorTrabajo.Medico.GetAll(includeProperties: "Especialidad");
                return Json(new { data = medicos }); // Asegúrate de que la respuesta sea un objeto JSON con la propiedad "data"
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de médicos");
                return Json(new { success = false, message = "Error al obtener la lista de médicos" });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var medicoDesdeBd = _contenedorTrabajo.Medico.Get(id);

            if (medicoDesdeBd == null)
            {
                return Json(new { success = false, message = "Error borrando médico: no se encontró el médico" });
            }

            // Elimina la imagen ANTES de eliminar el registro
            if (!string.IsNullOrEmpty(medicoDesdeBd.UrlImagen))
            {
                string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
                var rutaImagen = Path.Combine(rutaDirectorioPrincipal, medicoDesdeBd.UrlImagen.TrimStart('\\'));

                if (System.IO.File.Exists(rutaImagen))
                {
                    try
                    {
                        System.IO.File.Delete(rutaImagen);
                    }
                    catch (Exception ex)
                    {
                        // Manejar el error de eliminación de imagen (puedes registrarlo o notificarlo al usuario)
                        return Json(new { success = false, message = $"Error al eliminar la imagen: {ex.Message}" });
                    }
                }
            }

            // Ahora elimina el registro del médico
            _contenedorTrabajo.Medico.Remove(medicoDesdeBd);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Médico borrado correctamente" });
        }


    }

    #endregion
}
