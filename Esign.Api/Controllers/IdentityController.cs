using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Esign.Api.Controllers
{
    public class IdentityController : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("identity")]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return new JsonResult("test");
        }
    }
}

