using events.Data;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace events.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiContext _context;
        public ProfileController(ApiContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? regionid)
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                ViewBag.Title = "EventHive - Профиль";
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Cities = regionid != null ? [.. _context.Cities.Where(q => q.RegionId == regionid)] : null, Schools = [.. _context.Schools] };

                if (model.CurrentUser.RoleId < 3)
                    return View(model);
                else
                    return Redirect("~/Administration/Index");
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }

        public IActionResult GetLogonSettingPartial()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                ViewBag.Title = "EventHive - Профиль";
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users] };

                ViewBag.RoleName = _context.Roles.Single(q => q.Id == model.CurrentUser.RoleId).Name;

                return PartialView("LogonSetting", model);
            }
            else
            {
                return Redirect("~/Home/Index");
            } 
        }

        public IActionResult GetPersonalSettingPartial()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                ViewBag.Title = "EventHive - Профиль";
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users] };
                return PartialView("PersonalSetting", model);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public IActionResult GetUndertakenActivitiesPartial()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                try
                {
                    var jwt = Request.Cookies["eventhive"];
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwt);
                    var userId = token.Claims.First(c => c.Type == "userId").Value;
                    ViewBag.Title = "EventHive - Профиль";
                    var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], EventReviews = [.. _context.EventReviews] };

                    var userEventIds = _context.UserEvents.Where(q => q.UserId == int.Parse(userId)).Select(q => q.EventId).ToList();
                    var myevents = _context.Events.Where(ev => userEventIds.Contains(ev.Id)).ToList();

                    if (myevents.Where(q => q.DateTime.ToLocalTime() < DateTime.Now).OrderByDescending(q => q.DateTime).Count() > 0)
                    {
                        ViewBag.IsHaveEvs = true;
                        ViewBag.MyEvents = myevents.Where(q => q.DateTime.ToLocalTime() < DateTime.Now).OrderByDescending(q => q.DateTime);
                    }
                    else
                        ViewBag.IsHaveEvs = false;

                    return PartialView("UndertakenActivities", model);
                }
                catch
                {
                    return Redirect("~/Home/Error");
                }
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public IActionResult GetCommonSettingPartial()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                ViewBag.Title = "EventHive - Профиль";
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Schools = [.. _context.Schools] };

                ViewBag.School = _context.Schools.SingleOrDefault(q => q.UserId == int.Parse(userId));

                return PartialView("CommonSetting", model);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public IActionResult GetLocationSettingPartial()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                ViewBag.Title = "EventHive - Профиль";

                School school = _context.Schools.Single(q => q.UserId == int.Parse(userId));
                ViewBag.School = school;

                var city = _context.Cities.Single(q => q.Id == school.CityId);
                var region = _context.Regions.Single(q => q.Id == city.RegionId);
                ViewBag.Region = region.Name;
                ViewBag.City = city.Name;

                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Cities = [.. _context.Cities], Regions = [.. _context.Regions], Schools = [.. _context.Schools] };

                return PartialView("LocationSetting", model);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }
    }
}
