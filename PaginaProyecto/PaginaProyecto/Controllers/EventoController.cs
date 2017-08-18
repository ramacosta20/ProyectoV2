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

        public ActionResult HomeEventos()
        {
            Evento oEvento = new Evento();
            ViewBag.ListaEventos = oEvento.ListarEventos();
            return View();
        }
        public ActionResult Evento1()
        {
            return View();
        }
        public ActionResult Eventos()
        {
            return View();
        }


        // POST : Evento/AgregarEvento/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarEvento(Evento oEvento)
        {
            if (ModelState.IsValid)
            {
                if (oEvento.Imagen != null && oEvento.Imagen.ContentLength > 0)
                {
                    var filename = Path.GetFileName(oEvento.Imagen.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data"), filename);
                    oEvento.Imagen.SaveAs(path);
                    oEvento.InsertarEvento();
                }
                return View("Evento1");
            }
            else
            {
                return View();
            }
        }
    }
}