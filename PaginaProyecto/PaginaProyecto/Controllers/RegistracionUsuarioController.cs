using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaginaProyecto.Models;
using System.IO;

namespace PaginaProyecto.Controllers
{
    public class RegistracionUsuarioController : Controller
    {
        public ActionResult Desloguear()
        {
            Session["UsuarioLOgueado"] = null;
            return RedirectToAction("Index", "Home");
        }
        // GET: RegistracionUsuario/MiPerfil/
        public ActionResult MiPerfil()
        {
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            oUsuario.ListarEventosDono();
            ViewBag.Usuario = oUsuario;
            return View();
        }

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
                return RedirectToAction("MiPerfil", "RegistracionUsuario");
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
                return RedirectToAction("MiPerfil" , "RegistracionUsuario");
            }
            else
            {
                ViewBag.MensajeError = "El usuario o la contraseña no son correctos";
                return View();
            }
        }

        public ActionResult ModificarUsuario()
        {
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            ViewBag.Usuario = oUsuario; 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarUsuario(Usuario oUsuario)
        {
            Usuario logUsuario = (Usuario)Session["UsuarioLogueado"];
            oUsuario.UsuarioID = logUsuario.UsuarioID;
            oUsuario.Email = logUsuario.Email;
            oUsuario.Contraseña = logUsuario.Contraseña;
            if (oUsuario.Imagen != null && oUsuario.Imagen.ContentLength > 0)
            {
                var filename = Path.GetFileName(oUsuario.Imagen.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Usuarios"), filename);
                oUsuario.Imagen.SaveAs(path);
                oUsuario.ImagenString = oUsuario.Imagen.FileName; 
            }
            else
            {
                oUsuario.ImagenString = "default.gif";               
            }
            oUsuario.ModificarUsuario();
            Session["UsuarioLogueado"] = oUsuario;
            return RedirectToAction("MiPerfil");
        }
    }
}