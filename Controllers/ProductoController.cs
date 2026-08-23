using MichiMarket.Models;
using MichiMarket.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MichiMarket.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ProductoRepository _productoRepo;

        public ProductoController(ProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        public IActionResult Index(int pagina = 1, string? tipo = null, string? animal = null)
        {
            try
            {
                List<Producto> todos;
                if (!string.IsNullOrEmpty(tipo) || !string.IsNullOrEmpty(animal))
                {
                    todos = _productoRepo.Buscar(null, tipo, animal, null, null);
                }
                else
                {
                    todos = _productoRepo.Listar();
                }
                int tamanioPagina = 20;
                int totalPaginas = (int)Math.Ceiling((double)todos.Count / tamanioPagina);
                var productos = todos.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina).ToList();

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.FiltroTipo = tipo;
                ViewBag.FiltroAnimal = animal;
                ViewBag.EsAdmin = HttpContext.Session.GetString("rol") == "admin";
                return View(productos);
            }
            catch
            {
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
                ViewBag.EsAdmin = HttpContext.Session.GetString("rol") == "admin";
                return View(new List<Producto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buscar(string nombre, string tipo, string animal, decimal? precioMin, decimal? precioMax, int pagina = 1)
        {
            try
            {
                var productos = _productoRepo.Buscar(nombre, tipo, animal, precioMin, precioMax);

                int tamanioPagina = 20;
                int totalPaginas = (int)Math.Ceiling((double)productos.Count / tamanioPagina);
                var paginados = productos.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina).ToList();

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.FiltroNombre = nombre;
                ViewBag.FiltroTipo = tipo;
                ViewBag.FiltroAnimal = animal;
                ViewBag.FiltroPrecioMin = precioMin;
                ViewBag.FiltroPrecioMax = precioMax;
                ViewBag.EsBusqueda = true;
                ViewBag.EsAdmin = HttpContext.Session.GetString("rol") == "admin";
                return View("Index", paginados);
            }
            catch
            {
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
                ViewBag.EsBusqueda = true;
                ViewBag.EsAdmin = HttpContext.Session.GetString("rol") == "admin";
                return View("Index", new List<Producto>());
            }
        }

        public IActionResult Detalle(int id)
        {
            try
            {
                var producto = _productoRepo.ObtenerPorId(id);
                if (producto == null) return NotFound();
                return View(producto);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
