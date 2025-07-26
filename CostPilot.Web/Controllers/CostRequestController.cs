using Microsoft.AspNetCore.Mvc;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostRequest;
using static CostPilot.Common.ValidationErrorMessages;
using static CostPilot.Common.ApplicationConstants;
using Microsoft.AspNetCore.Authorization;

namespace CostPilot.Web.Controllers
{
    public class CostRequestController : BaseController
    {
        private readonly ICostCenterService costCenterService;
        private readonly ICostCurrencyService costCurrencyService;
        private readonly ICostTypeService costTypeService;
        private readonly ICostRequestService costRequestService;

        public CostRequestController(ICostCenterService costCenterService, 
            ICostCurrencyService costCurrencyService,
            ICostTypeService costTypeService,
            ICostRequestService costRequestService)
        {
            this.costCenterService = costCenterService;
            this.costCurrencyService = costCurrencyService;
            this.costTypeService = costTypeService;
            this.costRequestService = costRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
			try
			{
                var model = new CostRequestCreateInputModel();
                model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                model.Types = await costTypeService.GetActiveCostTypesAsync();
                return this.View(model);
			}
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CostRequestCreateInputModel model)
        {
            try
            {
                var costRequestAmount = 0.0m;
                var isAmountValid = decimal.TryParse(model.Amount, out costRequestAmount) == true && costRequestAmount > 0.0m;
                if (isAmountValid == false)
                {
                    this.ModelState.AddModelError(nameof(model.Amount), "Invalid Amount");   
                }

                if (this.ModelState.IsValid == false)
                {
                    model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                    model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                    model.Types = await costTypeService.GetActiveCostTypesAsync();
                    return this.View(model);
                }

                var userId = this.GetUserId();
                var createResult = await this.costRequestService.CreateCostRequestAsync(model, costRequestAmount, userId);
                if (createResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                    model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                    model.Types = await costTypeService.GetActiveCostTypesAsync();
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
        public async Task<IActionResult> Index(string? searchNumber, string? searchCurrency, string? searchStatus)
        {
            try
            {
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetMyCostRequestsAsync(userId, searchNumber, searchCurrency, searchStatus);
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            try
            {
                var model = await this.costRequestService.GetCostRequestDetailsAsync(id);
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

        [HttpGet]
        public async Task<IActionResult> Cancel(string? id)
        {
            try
            {
                var userId = this.GetUserId();
                var cancelResult = await this.costRequestService.CancelCostRequestAsync(id, userId);
                if (cancelResult == false)
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
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetCostRequestForEditAsync(id, userId);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }

                model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                model.Types = await costTypeService.GetActiveCostTypesAsync();
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CostRequestEditInputModel model)
        {
            try
            {
                var costRequestAmount = 0.0m;
                var isAmountValid = decimal.TryParse(model.Amount, out costRequestAmount) == true && costRequestAmount > 0.0m;
                if (isAmountValid == false)
                {
                    this.ModelState.AddModelError(nameof(model.Amount), "Invalid Amount");
                }

                if (this.ModelState.IsValid == false)
                {
                    model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                    model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                    model.Types = await costTypeService.GetActiveCostTypesAsync();
                    return this.View(model);
                }

                var userId = this.GetUserId();
                var editResult = await this.costRequestService.EditCostRequestAsync(model, costRequestAmount, userId);
                if (editResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Centers = await this.costCenterService.GetActiveCostCentersAsync();
                    model.Currencies = await this.costCurrencyService.GetActiveCostCurrenciesAsync();
                    model.Types = await costTypeService.GetActiveCostTypesAsync();
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
        public async Task<IActionResult> ForApproval()
        {
            try
            {
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetCostRequestsForApprovalAsync(userId);
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Approve(string? id)
        {
            try
            {
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetCostRequestForDecisionAsync(id, userId);
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
        public async Task<IActionResult> ConfirmApprove(CostRequestDecisionInputModel model)
        {
            try
            {
                var userId = this.GetUserId();
                var approveResult = await this.costRequestService.ApproveCostRequestAsync(model, userId);
                if (approveResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(ForApproval));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reject(string? id)
        {
            try
            {
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetCostRequestForDecisionAsync(id, userId);
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
        public async Task<IActionResult> ConfirmReject(CostRequestDecisionInputModel model)
        {
            try
            {
                var userId = this.GetUserId();
                var rejectResult = await this.costRequestService.RejectCostRequestAsync(model, userId);
                if (rejectResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(ForApproval));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AfterDecision(string? searchNumber, string? searchCurrency, string? searchStatus)
        {
            try
            {
                var userId = this.GetUserId();
                var model = await this.costRequestService.GetCostRequestsAfterDecisionAsync(userId, searchNumber, searchCurrency, searchStatus);
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        [Authorize(Roles = AnalystRole)]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var model = await this.costRequestService.GetDashboardStatisticsAsync();
                return this.View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }
    }
}
