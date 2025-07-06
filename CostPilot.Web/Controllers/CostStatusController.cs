using Microsoft.AspNetCore.Mvc;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostStatus;
using static CostPilot.Common.ValidationErrorMessages;
using static CostPilot.Common.ApplicationConstants;

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

        [HttpGet]
        public async Task<IActionResult> Disable(string? id)
        {
            try
            {
                var disableResult = await this.costStatusService.DisableCostStatusAsync(id);
                if (disableResult == false)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
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
        public async Task<IActionResult> Enable(string? id)
        {
            try
            {
                var enableResult = await this.costStatusService.EnableCostStatusAsync(id);
                if (enableResult == false)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
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
        public async Task<IActionResult> Edit(string? id)
        {
            try
            {
                var model = await this.costStatusService.GetCostStatusForEditAsync(id);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }

                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CostStatusEditInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var editResult = await this.costStatusService.EditCostStatusAsync(model);
                if (editResult == false) 
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
