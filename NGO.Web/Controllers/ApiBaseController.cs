using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NGO.Web.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class ApiBaseController : Controller
    {

    }
}
