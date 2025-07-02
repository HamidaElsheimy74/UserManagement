using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Helpers;

namespace UserManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected Language GetLanguage()
    {
        var language = Request.Headers["Accept-Language"];
        if (language == "en") return Language.En;
        else return Language.Hindi;
    }
}
