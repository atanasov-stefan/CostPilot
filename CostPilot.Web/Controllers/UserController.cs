using Microsoft.AspNetCore.Mvc;

using CostPilot.ViewModels.User;
using CostPilot.Services.Core.Contracts;
using static CostPilot.Common.ApplicationConstants;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await this.userService.GetUserFullDetailsAsync();
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string? id)
        {
            try
            {
                var model = await this.userService.GetUserForRoleAssignmentAsync(id);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }
                
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(UserAssignRoleInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    return this.View(model);
                }

                var assignResult = await this.userService.AssignRoleToUserAsync(model);
                if (assignResult == false)
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
