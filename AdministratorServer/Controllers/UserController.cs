
using AdministratorServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdministratorServer.Controllers
{
    [Route("api/v1/users")]
    public class UserController : Controller
    {
        [HttpPost]
        public IActionResult AddUser([FromBody] UserModel userModel)
        {
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpDelete("{email}")]
        public IActionResult DeleteAdministrator([FromRoute] string email)
        {
            return NoContent();
        }

        [HttpPut("{email}")]
        public IActionResult UpdateAdministrator([FromRoute] string email, [FromBody] UserUpdateModel userUpdateModel)
        {
            return NoContent();
        }
    }
}
