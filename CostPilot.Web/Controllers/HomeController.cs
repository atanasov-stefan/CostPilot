using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using CostPilot.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CostPilot.Web.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
