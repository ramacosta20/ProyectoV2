using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaginaProyecto.Models;

namespace PaginaProyecto.Controllers
{
    public class RegistracionAdminController : Controller
    {
        // GET: RegistracionAdmin/RegistrarAdmin
        public ActionResult RegistrarAdmin()
        {
            return View();
        }

        // POST: RegistracionAdmin/RegistrarAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarAdmin(Administrador oAdmin)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("");
            }
            else
            {
                return View();
            }
        }

        // GET: RegistracionAdmin/LoguearAdmin
        public ActionResult LoguearAdmin()
        {
            return View();
        }

        // POST: RegistracionAdmin/LoguearAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoguearAdmin(Administrador oAdmin)
        {
            if (ModelState.IsValid)
            {
                dbProyecto.Create(oAdmin);
                return RedirectToAction("HomeAdmin");
            }
            else
            {
                return View();
            }
        }
    }
}