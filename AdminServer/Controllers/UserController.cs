using System.Net;
using System.Threading.Tasks;
using AdminServer.Models;
using AdminServer.ServiceInterface;
using Domain;
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
        public async Task<IActionResult> AddUser([FromBody] UserModel userModel)
        {
            await _userService.AddUserAsync(userModel.ToEntity());
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteAdministrator([FromRoute] string email)
        {
            await _userService.DeleteUserAsync(new User { Email = email });
            return NoContent();
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateAdministrator([FromRoute] string email, [FromBody] UserUpdateModel userUpdateModel)
        {
            await _userService.UpdateUserAsync(userUpdateModel.ToEntity(email));
            return NoContent();
        }
    }
}
