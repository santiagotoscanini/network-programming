using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdminServer.Models;
using AdminServer.ServiceInterface;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] UserModel userModel)
        {
            _userService.AddUser(userModel.ToEntity());
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpDelete("{email}")]
        public IActionResult DeleteAdministrator([FromRoute] string email)
        {
            _userService.DeleteUser(new User { Email = email });
            return NoContent();
        }

        [HttpPut("{email}")]
        public IActionResult UpdateAdministrator([FromRoute] string email, [FromBody] UserUpdateModel userUpdateModel)
        {
            _userService.UpdateUser(userUpdateModel.ToEntity(email));
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetSomething()
        {
            return Ok();
        }
    }
}
