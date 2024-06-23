using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace BlogCore.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // Segunda versión página de inicio con paginación
        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            var medicos = _contenedorTrabajo.Medico.GetAll().AsQueryable();
           

            // Paginar los resultados
            var paginatedEntries = medicos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            

            // Crear el modelo para la vista
            HomeVM homeVM = new HomeVM()
            {
                Sliders = _contenedorTrabajo.Slider.GetAll(),
                ListMedicos = paginatedEntries,
                PageIndex = page,
                TotalPages = (int)System.Math.Ceiling((double)medicos.Count() / pageSize)
            };

            // Esta línea es para poder saber si estamos en el home o no
            ViewBag.IsHome = true;

            return View(homeVM);
        }

        // Para buscador
        [HttpGet]
        public IActionResult ResultadoBusqueda(string searchString, int page = 1, int pageSize = 3)
        {
            var medicos = _contenedorTrabajo.Medico.GetAll().AsQueryable();

            // Filtrar por título si hay un término de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                medicos = medicos.Where(e => e.Nombre.Contains(searchString));
            }

            // Paginar los resultados
            var paginatedEntries = medicos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Crear el modelo para la vista
            var model = new ListaPaginada<Medico>(paginatedEntries, medicos.Count(), page, pageSize, searchString);
            return View(model);
        }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            var articuloDesdeBd = _contenedorTrabajo.Medico.Get(id);
            if (articuloDesdeBd == null)
            {
                return NotFound();
            }
            return View(articuloDesdeBd);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
