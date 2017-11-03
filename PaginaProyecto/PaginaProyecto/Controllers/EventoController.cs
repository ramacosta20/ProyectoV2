using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using PaginaProyecto.Models;
using System.IO;

namespace PaginaProyecto.Controllers
{
    public class EventoController : Controller
    {
        public ActionResult MisEventos()
        {
            ViewBag.NoHayEventos = false;
            Evento oEvento = new Evento();
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            List<Evento> listaEventos = oEvento.ListarEventosUsuario(oUsuario.UsuarioID);
            if (listaEventos.Count > 0)
            {
                ViewBag.listaEventosUsuario = listaEventos;
            }
            else
            {
                ViewBag.NoHayEventos = true;
            }
            ViewBag.Usuario = oUsuario;
            return View();
        }
        // GET: Evento
        public ActionResult AgregarEvento()
        {
            ViewBag.Usuario = (Usuario)Session["UsuarioLogueado"];
            return View();
        }

        // POST : Evento/AgregarEvento/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarEvento(Evento oEvento)
        {
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            ViewBag.Usuario = oUsuario;
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
                    oEvento.UsuarioID = oUsuario.UsuarioID;
                    oEvento.InsertarEvento();
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
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            ViewBag.Usuario = oUsuario;
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
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            oEvento.recaudadoEvento();
            oEvento.CantidadDeDonantesEvento();
            oEvento.UsuarioAdmin.TraerUsuario();
            double porcen = 0;
            porcen = ((double)oEvento.Recaudado / (double)oEvento.Meta) * 100;
            int intporcen = Convert.ToInt32(Math.Floor(porcen));
            ViewBag.Porcentaje = intporcen;
            ViewBag.Usuario = oUsuario;
            ViewBag.unEvento = oEvento;
            return View();
        }

        public ActionResult ModificarEvento(int idEvento)
        {
            Evento oEvento = new Evento();
            oEvento.EventoID = idEvento;
            oEvento.TraerEvento();

            ViewBag.unEvento = oEvento;
            Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
            ViewBag.Usuario = oUsuario;
            return View();
        }

        // POST : Evento/ModificarEvento/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarEvento(Evento oEvento, int id)
        {
            oEvento.EventoID = id;
            if (oEvento.Imagen != null && oEvento.Imagen.ContentLength > 0)
            {
                var filename = Path.GetFileName(oEvento.Imagen.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Eventos"), filename);
                oEvento.Imagen.SaveAs(path);
                oEvento.ImagenString = oEvento.Imagen.FileName;  
            }
            else
            {
                oEvento.ImagenString = "default.gif";
            }
            oEvento.ModificarEvento();
            return RedirectToAction("MisEventos");
        }

        public ActionResult Donar(int idEvento)
        {
            ViewBag.EventoID = idEvento;
            Usuario ousuario = (Usuario)Session["Usuariologueado"];
            ViewBag.Usuario = ousuario;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donar(Donacion oDonacion, int idEvento)
        {
            if (ModelState.IsValid)
            {
                Evento oevento = new Evento();
                oevento.EventoID = idEvento;
                oevento.TraerEvento();
                oevento.recaudadoEvento();   
                Usuario oUsuario = (Usuario)Session["UsuarioLogueado"];
                int idUsuario = oUsuario.UsuarioID;
                if (oevento.Recaudado + oDonacion.Monto >= oevento.Meta)
                {
                    oevento.UsuarioAdmin.TraerUsuario();
                    MandarMail(oevento);
                }
                oDonacion.donar(idEvento, idUsuario);
                MandarMail1(oUsuario, oDonacion);
                return RedirectToAction("UnEvento", new { idEvento = idEvento});
            }
            else
            {
                Usuario ousuario = (Usuario)Session["Usuariologueado"];
                ViewBag.Usuario = ousuario;
                return View();
            }
        }
        public void MandarMail1 (Usuario oUsuario, Donacion oDonacion)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("uripaluch2@gmail.com");
            mailMessage.To.Add(oUsuario.Email);
            mailMessage.From = new MailAddress("proyectodonaciones@gmail.com");
            mailMessage.Subject = "Donacion aceptada";
            mailMessage.Body = "Aceptamos tu donacion de $"+oDonacion.Monto+" Muchas gracias por contribuir.";
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential("proyectodonaciones@gmail.com", "proyecto123");
            smtpClient.Port = 25;
            smtpClient.Send(mailMessage);
        }

        public void MandarMail(Evento oEvento)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(oEvento.UsuarioAdmin.Email);
            mailMessage.From = new MailAddress("proyectodonaciones@gmail.com");
            mailMessage.Subject = "Donacion aceptada";
            mailMessage.Body = "Se completo la meta de $" + oEvento.Meta;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential("proyectodonaciones@gmail.com", "proyecto123");
            smtpClient.Port = 25;
            smtpClient.Send(mailMessage);
        }
    }
}