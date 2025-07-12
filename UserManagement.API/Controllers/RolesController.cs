using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;
public class RolesController : ControllerBase
{
    private readonly IRoleServices _roleServices;

    public RolesController(IRoleServices roleServices)
    {
        _roleServices = roleServices;
    }

    [HttpGet("GetAllRoles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        var response = await _roleServices.GetAllRolesAsync();
        return Ok(response);
    }

    [HttpGet("GetRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRole([FromQuery] long roleId)
    {

        var response = await _roleServices.GetRoleAsync(roleId);
        return Ok(response);

    }

    [HttpPost("AddRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRole([FromBody] RoleDto roleDto)
    {

        var response = await _roleServices.CreateRoleAsync(roleDto);
        return Ok(response);

    }

    [HttpPut("UpdateRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole([FromBody] RoleDto roleDto)
    {

        var response = await _roleServices.UpdateRoleAsync(roleDto);
        return Ok(response);

    }

    [HttpDelete("DeleteRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole([FromQuery] long roleId)
    {

        var response = await _roleServices.DeleteRoleAsync(roleId);
        return Ok(response);

    }
}
