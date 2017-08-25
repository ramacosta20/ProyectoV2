using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaginaProyecto.Models;
using System.IO;

namespace PaginaProyecto.Controllers
{
    public class EventoController : Controller
    {
        // GET: Evento
        public ActionResult AgregarEvento()
        {
            return View();
        }

        // POST : Evento/AgregarEvento/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarEvento(Evento oEvento)
        {
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            bool nombreInvalido = oEvento.ExisteEvento();
            if (ModelState.IsValid && nombreInvalido == false)
            {
                if (oEvento.Imagen != null && oEvento.Imagen.ContentLength > 0)
                {
                    var filename = Path.GetFileName(oEvento.Imagen.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Eventos"), filename);
                    oEvento.Imagen.SaveAs(path);
                    oEvento.ImagenString = oEvento.Imagen.FileName;
                    oEvento.UsuarioID = oUsuario.UsuarioID;
                    oEvento.InsertarEvento();
                }
                else
                {
                    oEvento.ImagenString = "default.gif";
                }
                TempData["EventoCreado"] = oEvento;
                return RedirectToAction("UnEvento", new { idEvento = -1});
            }
            else
            {
                if (nombreInvalido)
                {
                    ViewBag.NombreUsado = "El nombre del evento ya fue usado";
                }
                return View();
            }
        }
        // GET : /Evento/HomeUsuario
        public ActionResult HomeEventos()
        {
            ViewBag.NoHayEventos = false;
            Evento oEvento = new Evento();
            List<Evento> listaEventos = oEvento.ListarEventos();
            if (listaEventos.Count > 0)
            {
                ViewBag.ListaEventos = listaEventos;
            }
            else
            {
                ViewBag.NoHayEventos = true;
            }
            ViewBag.Usuario = Session["UsuarioLogueado"];
            return View();
        }
        public ActionResult MisEventos()
        {
            return View();
        }

        public ActionResult UnEvento(int idEvento) {
            Evento oEvento = new Evento();
            if (idEvento != -1)
            {
                oEvento.EventoID = idEvento;
                oEvento.TraerEvento();              
            }
            else
            {
                oEvento = (Evento)TempData["EventoCreado"];
            }
            ViewBag.unEvento = oEvento;
            return View();
        }





    }
}