using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleServices _roleServices;
    private readonly ILogger<RolesController> _logger;
    public RolesController(IRoleServices roleServices, ILogger<RolesController> logger)
    {
        _roleServices = roleServices;
        _logger = logger;
    }

    [HttpGet("GetAllRoles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleServices.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("GetRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRole(long roleId)
    {

        var role = await _roleServices.GetRoleAsync(roleId);
        if (role == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        return Ok(role);

    }
}
