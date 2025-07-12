using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Models;

namespace UserManagement.API.Controllers;
public class AccountController : BaseController
{
    private readonly IAccountServices _accountServices;
    private readonly ILocalizer _localizer;
    public AccountController(IAccountServices accountServices,
                            ILocalizer localizer)
    {
        _accountServices = accountServices;
        _localizer = localizer;
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _accountServices.LoginUser(loginDto);
        return Ok(response);
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _accountServices.RegisterUser(registerDto);
        return Ok(response);
    }


    [HttpPost("RefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
    {
        var email = User?.Claims?.FirstOrDefault(obj => obj.Value == "Email")?.Value;
        if (email == null)
            return Unauthorized(new APIResponse(401, _localizer["InvalidUser"]));

        var response = await _accountServices.RefeshToken(model, email);
        return Ok(response);
    }
}
