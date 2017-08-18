using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaginaProyecto.Models;

namespace PaginaProyecto.Controllers
{
    public class RegistracionUsuarioController : Controller
    {
        // GET: RRegistracionUsuario/RegistrarUsuario/
        public ActionResult RegistrarUsuario()
        {
            return View();
        }

        // POST : RegistracionUsuario/RegistrarUsuario/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarUsuario(Usuario oUsuario)
        {
            oUsuario.ExisteMail();
            if (ModelState.IsValid && oUsuario.EmailValido)
            {
                oUsuario.InsertarUsuario();
                TempData["UsuarioLogueado"] = oUsuario;
                return RedirectToAction("HomeUsuario");
            }
            else
            {
                if (!oUsuario.EmailValido)
                {
                    ViewBag.EmailUsado = "El Email ingresado no esta disponible";
                }
                return View();
            }
        }

        // GET : RegistracionUsuario/LoguearUsuario
        public ActionResult LoguearUsuario()
        {
            return View();
        }

        // POST : RegistracionUsuario/LoguearUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoguearUsuario(Usuario oUsuario)
        {
            oUsuario.LoguearUsuario();
            if (oUsuario.Nombre != null)
            {
                TempData["UsuarioLogueado"] = oUsuario;
                return RedirectToAction("HomeUsuario");
            }
            else
            {
                ViewBag.MensajeError = "El usuario o la contraseña no son correctos";
                return View();
            }
        }

        // GET : /RegistracionUsuario/HomeUsuario
        public ActionResult HomeUsuario()
        {
            ViewBag.UsuarioLogueado = TempData["UsuarioLogueado"];
            return View();
        }
    }
}