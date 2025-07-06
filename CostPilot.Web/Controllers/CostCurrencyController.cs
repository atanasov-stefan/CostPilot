using Microsoft.AspNetCore.Mvc;

using CostPilot.Services.Core.Contracts;
using CostPilot.ViewModels.CostCurrency;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.Web.Controllers
{
    public class CostCurrencyController : BaseController
    {
        private readonly ICostCurrencyService costCurrencyService;

        public CostCurrencyController(ICostCurrencyService costCurrencyService)
        {
            this.costCurrencyService = costCurrencyService;   
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await this.costCurrencyService.GetCostCurrenciesAsync();
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
        public async Task<IActionResult> Create(CostCurrencyCreateInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var createResult = await this.costCurrencyService.CreateCostCurrencyAsync(model);
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
