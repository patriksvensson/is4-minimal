using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyIdentityServer.Controllers
{
    public class AdminController : Controller
    {
        [Authorize("admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
