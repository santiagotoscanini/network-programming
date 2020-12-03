﻿
using AdministratorServer.Models;
using AdministratorServer.ServicesInterface;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdministratorServer.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : Controller
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
