using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;
public class UserRolesController : BaseController
{
    private readonly IUserRoleServices _userRoleServices;
    public UserRolesController(IUserRoleServices userRoleServices)
    {
        _userRoleServices = userRoleServices;
    }


    [HttpGet("GetUserRoles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserRoles()
    {
        var response = await _userRoleServices.GetUserRolesAsync();
        return Ok(response);
    }

    [HttpGet("GetUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserRole([FromQuery] long roleId, long userId)
    {

        var response = await _userRoleServices.GetUserRoleAsync(userId, roleId);
        return Ok(response);

    }

    [HttpPost("AddUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserRole([FromBody] UserRoleDto userRoleDto)
    {

        var response = await _userRoleServices.AddUserRoleAsync(userRoleDto);
        return Ok(response);

    }

    [HttpPut("UpdateUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleDto userRoleDto)
    {

        var response = await _userRoleServices.UpdateUserRoleAsync(userRoleDto);
        return Ok(response);

    }

    [HttpDelete("DeleteUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserRole([FromQuery] long userId, long roleId)
    {

        var response = await _userRoleServices.DeleteUserRoleAsync(userId, roleId);
        return Ok(response);

    }
}
