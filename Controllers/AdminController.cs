using MichiMarket.Models;
using MichiMarket.Repositories;
using MichiMarket.Services;
using Microsoft.AspNetCore.Mvc;

namespace MichiMarket.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductoRepository _productoRepo;
        private readonly PedidoRepository _pedidoRepo;
        private readonly UsuarioRepository _usuarioRepo;
        private readonly ImagenService _imagenService;

        public AdminController(ProductoRepository productoRepo, PedidoRepository pedidoRepo,
            UsuarioRepository usuarioRepo, ImagenService imagenService)
        {
            _productoRepo = productoRepo;
            _pedidoRepo = pedidoRepo;
            _usuarioRepo = usuarioRepo;
            _imagenService = imagenService;
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetString("rol") == "admin";
        }

        public IActionResult Productos(int pagina = 1)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            var todos = _productoRepo.Listar();
            int tamanioPagina = 20;
            int totalPaginas = (int)Math.Ceiling((double)todos.Count / tamanioPagina);
            var productos = todos.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.EsAdmin = true;
            return View("~/Views/Producto/Index.cshtml", productos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                _productoRepo.Eliminar(id);
                TempData["Exito"] = "Producto eliminado";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar producto: " + ex.Message;
            }
            return RedirectToAction("Productos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarVarios(string ids)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                if (!string.IsNullOrEmpty(ids))
                    _productoRepo.EliminarVarios(ids);
                TempData["Exito"] = "Productos eliminados";
            }
            catch { TempData["Error"] = "Error al eliminar productos"; }
            return RedirectToAction("Productos");
        }

        public IActionResult CrearProducto()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> CrearProducto(Producto p, IFormFile? foto)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                p.estado = true;
                if (foto != null && foto.Length > 0)
                    p.foto_producto = await _imagenService.SubirImagen(foto, "productos");
                else
                    p.foto_producto = "img/gato_cara.png";

                _productoRepo.Guardar(p);
                TempData["Exito"] = "Producto creado correctamente";
                return RedirectToAction("Productos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear producto: " + ex.Message;
                return View(p);
            }
        }

        public IActionResult EditarProducto(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            var producto = _productoRepo.ObtenerPorId(id);
            if (producto == null) return RedirectToAction("Productos");
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> EditarProducto(Producto p, string estadoStr, IFormFile? foto)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                p.estado = estadoStr == "true";
                if (foto != null && foto.Length > 0)
                    p.foto_producto = await _imagenService.SubirImagen(foto, "productos");
                else if (string.IsNullOrEmpty(p.foto_producto))
                    p.foto_producto = "img/gato_cara.png";

                _productoRepo.Guardar(p);
                TempData["Exito"] = "Producto actualizado correctamente";
                return RedirectToAction("Productos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                return View(p);
            }
        }
        public IActionResult Usuarios()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                var usuarios = _usuarioRepo.Listar();
                return View(usuarios);
            }
            catch
            {
                TempData["Error"] = "Error al listar usuarios";
                return View(new List<Usuario>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarUsuario(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try { _usuarioRepo.Eliminar(id); TempData["Exito"] = "Usuario eliminado"; } catch { TempData["Error"] = "Error al eliminar usuario"; }
            return RedirectToAction("Usuarios");
        }
        public IActionResult Pedidos()
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                var pedidos = _pedidoRepo.ListarTodos();
                return View(pedidos);
            }
            catch
            {
                TempData["Error"] = "Error al listar pedidos";
                return View(new List<Pedido>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstadoPedido(int idPedido, string estado)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try { _pedidoRepo.ActualizarEstado(idPedido, estado); TempData["Exito"] = "Estado actualizado"; } catch { TempData["Error"] = "Error al actualizar estado"; }
            return RedirectToAction("Pedidos");
        }

        public IActionResult DetallePedido(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Index", "Home");
            try
            {
                var detalles = _pedidoRepo.DetallePedidoAdmin(id);
                ViewBag.IdPedido = id;
                return View("~/Views/Pedido/Detalle.cshtml", detalles);
            }
            catch
            {
                TempData["Error"] = "Error al cargar el detalle del pedido";
                return RedirectToAction("Pedidos");
            }
        }
    }
}
