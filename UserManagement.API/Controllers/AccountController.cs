using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
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
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
    {

        var response = await _accountServices.RefeshToken(model);
        return Ok(response);
    }
}
