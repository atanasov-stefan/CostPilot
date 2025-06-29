using Microsoft.AspNetCore.Mvc;

namespace CostPilot.Web.Controllers
{
    public class CostCenterController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
