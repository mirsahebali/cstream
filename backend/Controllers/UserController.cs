using Microsoft.AspNetCore.Mvc;

using Cstream.Models;
using Cstream.Services;

namespace Cstream.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController() { }

    [HttpGet("{username}")]
    public ActionResult<User?> Get(string username)
    {
        User? foundUser = UserService.Get(username);
        if (foundUser == null)
        {
            return NotFound("user " + username + " not found");
        }

        return foundUser;
    }

    [HttpPost]
    public IActionResult Post(string username)
    {
        User? created = UserService.CreateUser(username);
        if (created == null)
        {
            return BadRequest("unable to create user");
        }

        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete(string username)
    {

        return Ok();
    }

}
