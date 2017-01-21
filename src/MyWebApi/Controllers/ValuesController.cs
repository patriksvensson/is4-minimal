using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{
    public class ValuesController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/public")]
        public IEnumerable<string> Open()
        {
            return new[] { "public1", "public2" };
        }

        [HttpGet]
        [Authorize("internal")]
        [Route("api/internal")]
        public IEnumerable<string> Internal()
        {
            return new[] { "internal1", "internal2" };
        }

        [HttpGet]
        [Authorize("thirdparty")]
        [Route("api/thirdparty")]
        public IEnumerable<string> External()
        {
            return new[] { "thirdparty1", "thirdparty2" };
        }
    }
}
