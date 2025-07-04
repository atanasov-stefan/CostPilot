﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CostPilot.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected RedirectToActionResult ExceptionCatchRedirect()
        {
            return this.RedirectToAction("Index", "Home");
        }
    }
}
