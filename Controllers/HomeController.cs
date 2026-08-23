using MichiMarket.Models;
using MichiMarket.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MichiMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductoRepository _productoRepo;

        public HomeController(ProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        public IActionResult Index()
        {
            try
            {
                var todos = _productoRepo.Listar();
                var destacados = todos
                    .Where(p => p.estado)
                    .OrderByDescending(p => p.precio)
                    .Take(4)
                    .ToList();
                return View(destacados);
            }
            catch
            {
                return View(new List<Producto>());
            }
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
