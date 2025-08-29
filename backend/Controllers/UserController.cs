using Microsoft.AspNetCore.Mvc;

using Cstream.Models;
using Cstream.Services;
using Cstream.Data;

namespace Cstream.Controllers;

public record class UserCreateInput
{
    public string Username;
    public string Password;
}

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{


    private UserService _service;

    public UserController(CstreamContext context)
    {
        _service = new UserService(context);
    }

    // INFO: removed for security purposes
    // [HttpGet("{username}")]
    // public ActionResult<User> Get(string username)
    // {
    //     User foundUser = _service.Get(username);
    //     if (foundUser == null)
    //     {
    //         return NotFound("user " + username + " not found");
    //     }
    //
    //     foundUser.HashedPassword = "[REDACTED]";
    //
    //     return Ok(foundUser);
    // }

    [HttpPost]
    public async Task<IActionResult> Post(UserCreateInput input)
    {
        User created = await _service.CreateUser(input.Username, input.Password);
        if (created == null)
        {
            return BadRequest(new { Message = "unable to create a new user", Error = true });
        }

        return Ok(new { Message = "successfully created user", Error = false });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string username)
    {
        bool is_removed = await _service.RemoveUser(username);
        if (!is_removed)
        {
            return NotFound("user not found");
        }

        return Ok("deleted user");
    }

}
