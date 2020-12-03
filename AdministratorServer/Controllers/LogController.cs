using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AdministratorServer.Controllers
{
    [Route("api/v1/logs")]
    public class LogController : Controller
    {
        [HttpGet]
        public IActionResult AddUser()
        {
            return Ok();
        }

    }
}
