using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserServices _userServices;
    private readonly ILogger<UserController> _logger;
    public UserController(IUserServices userServices, ILogger<UserController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpGet("GetAllUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllUsers()
    {

        var users = await _userServices.GetAllUsersAsync();
        return Ok(users);

    }

    [HttpPost("AddUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {

        var users = await _userServices.CreateUserAsync(user);
        return Ok(users);

    }
}
