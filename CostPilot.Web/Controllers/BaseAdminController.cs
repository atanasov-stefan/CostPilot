using Microsoft.AspNetCore.Authorization;

using static CostPilot.Common.ApplicationConstants;

namespace CostPilot.Web.Controllers
{
    [Authorize(Roles = AdminRole)]
    public abstract class BaseAdminController : BaseController
    {
    }
}
