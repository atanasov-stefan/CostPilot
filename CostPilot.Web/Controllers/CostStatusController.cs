using Microsoft.AspNetCore.Mvc;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostStatus;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.Web.Controllers
{
    public class CostStatusController : BaseController
    {
        private readonly ICostStatusService costStatusService;

        public CostStatusController(ICostStatusService costStatusService)
        {
            this.costStatusService = costStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await this.costStatusService.GetCostStatusesAsync();
                return this.View(model);
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
        public async Task<IActionResult> Create(CostStatusCreateInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var createResult = await this.costStatusService.CreateCostStatusAsync(model);
                if (createResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
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
    }
}
