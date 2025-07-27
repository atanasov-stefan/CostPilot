using Microsoft.AspNetCore.Authorization;

using static CostPilot.Common.ApplicationConstants;

namespace CostPilot.Web.Controllers
{
    [Authorize(Roles = UserRole)]
    public abstract class BaseUserController : BaseController
    {
    }
}
