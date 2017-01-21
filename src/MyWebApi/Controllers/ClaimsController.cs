using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Json(User.Claims.Select(claim => new
            {
                type = claim.Type,
                value = claim.Value
            }));
        }
    }
}
