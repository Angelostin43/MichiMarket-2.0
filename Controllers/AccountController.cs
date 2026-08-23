using MichiMarket.Models;
using MichiMarket.Repositories;
using MichiMarket.Services;
using Microsoft.AspNetCore.Mvc;

namespace MichiMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly ImagenService _imagenService;

        public AccountController(UsuarioRepository usuarioRepo, ImagenService imagenService)
        {
            _usuarioRepo = usuarioRepo;
            _imagenService = imagenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string correo, string password)
        {
            try
            {
                var usuario = _usuarioRepo.IniciarSesion(correo, password);
                if (usuario == null)
                {
                    ViewBag.Error = "Correo o contrasena incorrectos";
                    return View();
                }

                HttpContext.Session.SetInt32("id_usuario", usuario.id_usuario);
                HttpContext.Session.SetString("nombres", usuario.nombres);
                HttpContext.Session.SetString("rol", usuario.rol);
                HttpContext.Session.SetString("foto_perfil", usuario.foto_perfil ?? "");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.Error = "Error al iniciar sesion";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Registro(Usuario u, IFormFile? foto_perfil)
        {
            try
            {
                if (foto_perfil != null && foto_perfil.Length > 0)
                    u.foto_perfil = await _imagenService.SubirImagen(foto_perfil, "perfiles");
                else
                    u.foto_perfil = "";

                _usuarioRepo.Guardar(u);

                var usuario = _usuarioRepo.ObtenerPorCorreo(u.correo);
                if (usuario != null)
                {
                    HttpContext.Session.SetInt32("id_usuario", usuario.id_usuario);
                    HttpContext.Session.SetString("nombres", usuario.nombres);
                    HttpContext.Session.SetString("rol", usuario.rol);
                    HttpContext.Session.SetString("foto_perfil", u.foto_perfil ?? "");
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(u);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            var id = HttpContext.Session.GetInt32("id_usuario");
            if (id == null) return RedirectToAction("Login");

            var usuario = _usuarioRepo.ObtenerPorId(id.Value);
            if (usuario == null) return RedirectToAction("Index", "Home");

            usuario.password = "";
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Perfil(Usuario u, IFormFile? foto_perfil)
        {
            var id = HttpContext.Session.GetInt32("id_usuario");
            if (id == null) return RedirectToAction("Login");

            try
            {
                u.id_usuario = id.Value;

                if (foto_perfil != null && foto_perfil.Length > 0)
                {
                    var nuevaRuta = await _imagenService.SubirImagen(foto_perfil, "perfiles");
                    if (nuevaRuta != null)
                        u.foto_perfil = nuevaRuta;
                }
                else
                {
                    u.foto_perfil = HttpContext.Session.GetString("foto_perfil") ?? "";
                }

                if (string.IsNullOrEmpty(u.password))
                {
                    var actual = _usuarioRepo.ObtenerPorId(id.Value);
                    u.password = actual?.password ?? "";
                }

                _usuarioRepo.Guardar(u);

                HttpContext.Session.SetString("nombres", u.nombres);
                HttpContext.Session.SetString("foto_perfil", u.foto_perfil);

                TempData["Exito"] = "Perfil actualizado correctamente";
                u.password = "";
                return View(u);
            }
            catch
            {
                TempData["Error"] = "Error al actualizar el perfil";
                var usuario = _usuarioRepo.ObtenerPorId(id.Value);
                if (usuario != null) usuario.password = "";
                return View(usuario ?? u);
            }
        }
    }
}
