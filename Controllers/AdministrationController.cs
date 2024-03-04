using events.Data;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace events.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly ApiContext _context;
        public AdministrationController(ApiContext context) 
        { 
            _context = context;
        }
        public IActionResult Index()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                ViewBag.Title = "EventHive - Администрирование";
                ViewBag.EventReviews = _context.EventReviews.OrderBy(q => q.Id).ToList();
                ViewBag.Events = _context.Events.OrderBy(q => q.Id).ToList();

                ViewBag.Regions = _context.Regions.OrderBy(q => q.Id).ToList();
                ViewBag.Cities = _context.Cities.OrderBy(q => q.Id).ToList();

                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Schools = _context.Schools.OrderBy(q => q.Id).ToList() };

                if (model.CurrentUser.RoleId == 3)
                    return View(model);
                else
                    return Redirect("~/Administration/Index");
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }

        [HttpGet]
        public ActionResult CreateSchool(int userid)
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                return PartialView("CreateSchool");
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }
    }
}
