using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Utiles
{
    public class RequireContextAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (ctx.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                var controller = ctx.ActionDescriptor.ControllerDescriptor.ControllerName;
                var action = ctx.ActionDescriptor.ActionName;

                // No bloquear la pantalla de selección
                if (controller == "Contexto" && action == "Seleccionar")
                    return;

                if (string.IsNullOrEmpty(WebSession.Local))
                {
                    var url = new UrlHelper(ctx.RequestContext).Action("Seleccionar", "Contexto");
                    ctx.Result = new RedirectResult(url);
                    return;
                }
            }
            base.OnActionExecuting(ctx);
        }
    }
}