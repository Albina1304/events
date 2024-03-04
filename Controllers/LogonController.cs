using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    public class LogonController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Registration() => View();
        public IActionResult Recovery() => View();
        public EmptyResult Logout()
        {
            Response.Cookies.Delete("eventhive");
            return new EmptyResult();
        }
    }
}
