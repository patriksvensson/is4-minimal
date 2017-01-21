using Microsoft.AspNetCore.Mvc;

namespace MyMvcSite.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Logout()
        {
            HttpContext.Authentication.SignOutAsync("cookie");

            return RedirectToAction("Index", "Home");
        }
    }
}
