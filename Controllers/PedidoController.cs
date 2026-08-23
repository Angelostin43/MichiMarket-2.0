using MichiMarket.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MichiMarket.Controllers
{
    public class PedidoController : Controller
    {
        private readonly PedidoRepository _pedidoRepo;

        public PedidoController(PedidoRepository pedidoRepo)
        {
            _pedidoRepo = pedidoRepo;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RealizarPedido()
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Login", "Account");

            try
            {
                var mensaje = _pedidoRepo.RealizarPedido(idUsuario.Value);
                if (mensaje == "Pedido realizado con exito")
                    TempData["Exito"] = mensaje;
                else
                    TempData["Error"] = mensaje;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al realizar el pedido: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Login", "Account");

            try
            {
                var pedidos = _pedidoRepo.ListarMisPedidos(idUsuario.Value);
                return View(pedidos);
            }
            catch
            {
                TempData["Error"] = "Error al cargar los pedidos";
                return View(new List<Models.Pedido>());
            }
        }

        public IActionResult Detalle(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usuario");
            if (idUsuario == null) return RedirectToAction("Login", "Account");

            try
            {
                var detalles = _pedidoRepo.DetallePedido(idUsuario.Value, id);
                return View(detalles);
            }
            catch
            {
                TempData["Error"] = "Error al cargar el detalle del pedido";
                return View(new List<Models.PedidoDetalle>());
            }
        }
    }
}
