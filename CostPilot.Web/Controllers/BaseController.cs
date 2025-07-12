using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CostPilot.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected RedirectToActionResult ExceptionCatchRedirect()
        {
            return this.RedirectToAction("Index", "Home");
        }

        protected string GetUserId()
        {
            return this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
