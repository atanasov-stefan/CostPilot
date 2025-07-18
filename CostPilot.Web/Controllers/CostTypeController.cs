﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostType;
using static CostPilot.Common.ValidationErrorMessages;
using static CostPilot.Common.ApplicationConstants;

namespace CostPilot.Web.Controllers
{
    public class CostTypeController : BaseController
    {
        private readonly ICostTypeService costTypeService;

        public CostTypeController(ICostTypeService costTypeService)
        {
            this.costTypeService = costTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await this.costTypeService.GetCostTypesAsync();
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
        public async Task<IActionResult> Create(CostTypeCreateInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var createResult = await this.costTypeService.CreateCostTypeAsync(model);
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
                var disableResult = await this.costTypeService.DisableCostTypeAsync(id);
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
                var enableResult = await this.costTypeService.EnableCostTypeAsync(id);
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
                var model = await this.costTypeService.GetCostTypeForEditAsync(id);
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
        public async Task<IActionResult> Edit(CostTypeEditInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var editResult = await costTypeService.EditCostTypeAsync(model);
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
