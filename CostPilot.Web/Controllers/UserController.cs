using Microsoft.AspNetCore.Mvc;

using CostPilot.ViewModels.User;
using CostPilot.Services.Core.Contracts;
using static CostPilot.Common.ApplicationConstants;
using static CostPilot.Common.ValidationErrorMessages;

namespace CostPilot.Web.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public UserController(IUserService userService,
            IRoleService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
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
                var model = await this.userService.GetUserForRoleAssignmentOrRemovalAsync(id);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }

                model.Roles = await this.roleService.GetAllRolesExceptUserRolesAsync(model.Id);
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(UserRoleInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    model.Roles = await this.roleService.GetAllRolesExceptUserRolesAsync(model.Id);
                    return this.View(model);
                }

                var assignResult = await this.userService.AssignRoleToUserAsync(model);
                if (assignResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Roles = await this.roleService.GetAllRolesExceptUserRolesAsync(model.Id);
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
        public async Task<IActionResult> RemoveRole(string? id)
        {
            try
            {
                var model = await this.userService.GetUserForRoleAssignmentOrRemovalAsync(id);
                if (model == null)
                {
                    this.Response.StatusCode = 400;
                    return this.View(PathToBadRequestView);
                }

                model.Roles = await this.roleService.GetUserRolesAsync(model.Id);
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.ExceptionCatchRedirect();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(UserRoleInputModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    model.Roles = await this.roleService.GetUserRolesAsync(model.Id);
                    return this.View(model);
                }

                var removeResult = await this.userService.RemoveRoleFromUserAsync(model);
                if (removeResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, CreateEditOverallErrorMessage);
                    model.Roles = await this.roleService.GetUserRolesAsync(model.Id);
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
