using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminServer.Models;
using AdminServer.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var logs = _logService.GetLogs().Select(l => new LogModel { Text = l });
            return Ok(logs);
        }
    }
}
