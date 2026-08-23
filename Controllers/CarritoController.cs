using MichiMarket.Models;
using MichiMarket.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MichiMarket.Controllers
{
    public class CarritoController : Controller
    {
        private readonly CarritoRepository _carritoRepo;
        private readonly ProductoRepository _productoRepo;

        public CarritoController(CarritoRepository carritoRepo, ProductoRepository productoRepo)
        {
            _carritoRepo = carritoRepo;
            _productoRepo = productoRepo;
        }

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null)
            {
                return View(new List<CarritoDetalle>());
            }

            try
            {
                var items = _carritoRepo.Listar(idUsuario.Value);
                return View(items);
            }
            catch
            {
                TempData["Error"] = "Error al cargar el carrito";
                return View(new List<CarritoDetalle>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(int idProducto, int cantidad = 1)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = false, redirect = Url.Action("Login", "Account") });
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _carritoRepo.Agregar(idUsuario.Value, idProducto, cantidad);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = true, mensaje = "Agregado al carrito" });

                return RedirectToAction("Index");
            }
            catch
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = false, mensaje = "Error al agregar al carrito" });
                TempData["Error"] = "Error al agregar al carrito";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int idCarritoDetalle)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Login", "Account");

            try { _carritoRepo.Eliminar(idUsuario.Value, idCarritoDetalle); }
            catch { TempData["Error"] = "Error al eliminar el item del carrito"; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCantidad(int idCarritoDetalle, int cantidad)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = false });
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (cantidad <= 0)
                {
                    _carritoRepo.Eliminar(idUsuario.Value, idCarritoDetalle);
                }
                else
                {
                    _carritoRepo.ActualizarCantidad(idUsuario.Value, idCarritoDetalle, cantidad);
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = true });

                return RedirectToAction("Index");
            }
            catch
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { ok = false, mensaje = "Error al actualizar cantidad" });
                TempData["Error"] = "Error al actualizar cantidad";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FusionarCarrito([FromBody] List<CarritoLocalItem> items)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null)
                return Json(new { ok = false });

            try
            {
                foreach (var item in items)
                {
                    _carritoRepo.Agregar(idUsuario.Value, item.id_producto, item.cantidad);
                }
                return Json(new { ok = true });
            }
            catch
            {
                return Json(new { ok = false });
            }
        }
    }

    public class CarritoLocalItem
    {
        public int id_producto { get; set; }
        public int cantidad { get; set; }
    }
}
