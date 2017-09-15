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
        // GET: RegistracionUsuario/RegistrarUsuario/
        public ActionResult RegistrarUsuario()
        {
            return View();
        }

        // POST : RegistracionUsuario/RegistrarUsuario/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarUsuario(Usuario oUsuario)
        {
            //si el mail es valido hace que la propiedad de la clase EmailValido(bool) sea true
            oUsuario.ExisteMail();
            if (ModelState.IsValid && oUsuario.EmailValido)
            {
                //si llega una imagen como parametro la guardo en Content/Usuarios y guardo el nombre del archivo en la DB
                if (oUsuario.Imagen != null)
                {
                    string NuevaUbicacion = Server.MapPath("~/Content/Usuarios/") + oUsuario.Imagen.FileName;
                    oUsuario.Imagen.SaveAs(NuevaUbicacion);
                    oUsuario.ImagenString = oUsuario.Imagen.FileName;
                }
                else
                {
                    //si no llega una imagen como parametro asigno Default.png al usuario
                    oUsuario.ImagenString = "Default.png";
                }
                oUsuario.InsertarUsuario();
                Session["UsuarioLogueado"] = oUsuario;
                return RedirectToAction("HomeEventos", "Evento");
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
                Session["UsuarioLogueado"] = oUsuario;
                return RedirectToAction("HomeEventos" , "Evento");
            }
            else
            {
                ViewBag.MensajeError = "El usuario o la contraseña no son correctos";
                return View();
            }
        }
    }
}