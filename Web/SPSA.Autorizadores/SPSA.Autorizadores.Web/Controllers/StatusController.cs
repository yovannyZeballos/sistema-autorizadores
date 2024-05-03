using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Controllers
{
	public class StatusController : Controller
	{
		// GET: Status
		public JsonResult Index()
		{
			var respuesta = new RespuestaComunDTO
			{
				Ok = true,
				Mensaje = "Servicio en linea"
			};

			return Json(respuesta, JsonRequestBehavior.AllowGet);
		}
	}
}