using Microsoft.AspNetCore.Mvc;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCenter;
using static CostPilot.Common.ApplicationConstants;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.Web.Controllers
{
    public class CostCenterController : BaseController
    {
        private readonly ICostCenterService costCenterService;
        private readonly IUserService userService;

        public CostCenterController(ICostCenterService costCenterService, IUserService userService)
        {
            this.costCenterService = costCenterService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await this.costCenterService.GetCostCentersAsync();
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
                var model = new CostCenterCreateInputModel();
                model.Owners = await this.userService.GetUserDetailsAsync();
                return this.View(model);
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
                    model.Owners = await this.userService.GetUserDetailsAsync();
                    return this.View(model);
                }

                var createResult = await this.costCenterService.CreateCostCenterAsync(model);
                if (createResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Owners = await this.userService.GetUserDetailsAsync();
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
                var disableResult = await this.costCenterService.DisableCostCenterAsync(id);
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
                var enableResult = await this.costCenterService.EnableCostCenterAsync(id);
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
                var model = await this.costCenterService.GetCostCenterForEditAsync(id);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }

                model.Owners = await this.userService.GetUserDetailsAsync();
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CostCenterEditInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    model.Owners = await this.userService.GetUserDetailsAsync();
                    return this.View(model);
                }

                var editResult = await this.costCenterService.EditCostCenterAsync(model);
                if (editResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Owners = await this.userService.GetUserDetailsAsync();
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
