using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCenter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace CostPilot.Web.Controllers
{
    public class CostCenterController : BaseController
    {
        private readonly ICostCenterService costCenterService;

        public CostCenterController(ICostCenterService costCenterService)
        {
            this.costCenterService = costCenterService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var models = await this.costCenterService.GetCostCentersAsync();
                return this.View(models);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                return this.View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Create(CostCenterCreateInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var createResult = await this.costCenterService.CreateCostCenterAsync(model);
                if (createResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Assigned cost center code or description already existing!");
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            try
            {
                var deleteResult = await this.costCenterService.DeleteCostCenterAsync(id);
                if (deleteResult == false) 
                {
                    //TODO: Recheck
                    return this.View("~/Views/Shared/BadRequest.cshtml");
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        private RedirectToActionResult ExceptionCatchRedirect()
        { 
            return this.RedirectToAction("Index", "Home");
        }
    }
}
