using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Admin")]
    public class PacientesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<MedicosController> _logger;

        public PacientesController(IContenedorTrabajo contenedorTrabajo,
            IWebHostEnvironment hostingEnvironment, ILogger<MedicosController> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var pacientes = _contenedorTrabajo.Paciente.GetAll();
            return View(pacientes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var pacienteVM = new PacienteVM
            {
                Paciente = new Paciente(),
                
            };

            return View(pacienteVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PacienteVM pacienteVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (pacienteVM.Paciente.Id == 0 && archivos.Count > 0)
                {
                    // Nuevo paciente con imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\pacientes");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    pacienteVM.Paciente.UrlImagen = @"\imagenes\pacientes\" + nombreArchivo + extension;

                    _contenedorTrabajo.Paciente.Add(pacienteVM.Paciente);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Imagen", "Debes seleccionar una imagen");
                }
            }

            return View("Create", pacienteVM); // Aquí debes devolver la vista "Create" con el modelo correcto
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var pacienteVM = new PacienteVM
            {
                Paciente = new Paciente(), // Inicializa un nuevo paciente para evitar NullReferenceException
            };

            if (id == null)
            {
                return NotFound();
            }

            // Obtén el paciente de la base de datos usando el id proporcionado
            pacienteVM.Paciente = _contenedorTrabajo.Paciente.Get(id.Value);

            if (pacienteVM.Paciente == null)
            {
                return NotFound();
            }

            return View(pacienteVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PacienteVM pacienteVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                // Obtén el paciente desde la base de datos usando el id del paciente en el modelo pacienteVM
                var pacienteDesdeBd = _contenedorTrabajo.Paciente.Get(pacienteVM.Paciente.Id);

                if (pacienteDesdeBd == null)
                {
                    return NotFound();
                }

                if (archivos.Count > 0)
                {
                    // Procesar la imagen si se ha cargado un archivo nuevo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subida = Path.Combine(rutaPrincipal, @"imagenes\pacientes");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subida, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    pacienteVM.Paciente.UrlImagen = @"\imagenes\pacientes\" + nombreArchivo + extension;
                }
                else
                {
                    // Conservar la imagen existente si no se sube una nueva
                    pacienteVM.Paciente.UrlImagen = pacienteDesdeBd.UrlImagen;
                }

                // Actualizar el paciente en la base de datos
                _contenedorTrabajo.Paciente.Update(pacienteVM.Paciente);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, vuelve a cargar la vista de edición con el modelo pacienteVM
            return View(pacienteVM);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Paciente.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var pacienteDesdeBd = _contenedorTrabajo.Paciente.Get(id);
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, pacienteDesdeBd.UrlImagen.TrimStart('\\'));
            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }


            if (pacienteDesdeBd == null)
            {
                return Json(new { success = false, message = "Error borrando Paciente" });
            }

            _contenedorTrabajo.Paciente.Remove(pacienteDesdeBd);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Paciente Borrado Correctamente" });
        }

        #endregion
    
    }
}

