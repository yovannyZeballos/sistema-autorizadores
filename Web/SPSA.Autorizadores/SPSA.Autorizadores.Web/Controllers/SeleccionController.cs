using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Controllers
{
    public class SeleccionController : Controller
    {
        [HttpGet]
        public ActionResult Ubicacion()
        {
            if (WebSession.Login == null)
                return RedirectToAction("Index", "Login");

            return View();
        }
    }
}