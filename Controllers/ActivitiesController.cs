using events.Data;
using events.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace events.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApiContext _context;
        public ActivitiesController(ApiContext context) 
        {
            _context = context;
        }
        public IActionResult Activity(int activityid)
        {
            try
            {
                string? jwtToken = Request.Cookies["eventhive"];

                if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
                {
                    var jwt = Request.Cookies["eventhive"];
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwt);
                    var userId = token.Claims.First(c => c.Type == "userId").Value;

                    var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Schools = [.. _context.Schools] };
                    ViewBag.User = model.CurrentUser;

                    Event ev = _context.Events.Single(m => m.Id == activityid);
                    if (ev != null)
                    {
                        var school = _context.Schools.Single(q => q.Id == ev.SchoolId);

                        ViewBag.SchoolName = school.Name;
                        ViewBag.SchoolAddress = _context.Regions.Single(q => q.Id == _context.Cities.Single(q => q.Id == school.CityId).RegionId).Name + ", " + _context.Cities.Single(q => q.Id == school.CityId).Name + ", " + school.Street + ", " + school.House;
                        ViewBag.SchoolEmail = model.Users.First(q => q.Id == school.UserId).Email;
                        ViewBag.Event = ev;
                        ViewBag.EventReviews = _context.EventReviews.Where(q => q.EventId == ev.Id).ToList();

                        ViewBag.SchoolId = school.Id;

                        if (ev.DateTime > DateTime.Now.ToLocalTime())
                        {
                            UserEvent? userEvent = _context.UserEvents.SingleOrDefault(q => q.UserId == int.Parse(userId) && q.EventId == activityid);
                            if (userEvent != null)
                                ViewBag.AlreadyRegistered = true;
                            else
                                ViewBag.AlreadyRegistered = false;
                            ViewBag.IsExpired = false;
                        }
                        else
                        {
                            ViewBag.IsExpired = true;
                        }
                    }

                    ViewBag.Title = $"EventHive - {ev.Name}";

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
            catch
            {
                return Redirect("~/Home/Error");
            }
        }

        [HttpGet]
        public ActionResult CreateActivity(int schoolid)
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                ViewBag.School = _context.Schools.SingleOrDefault(q => q.Id == schoolid);

                if (ViewBag.School != null && ViewBag.School.UserId == int.Parse(userId))
                {
                    return PartialView("CreateActivity");
                }
                else
                    return Redirect("~/Home/Error");
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }

        [HttpGet]
        public PartialViewResult CreateReview(int eventid)
        {
            string jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                ViewBag.UserId = int.Parse(userId);
                ViewBag.EventId = eventid;

                return PartialView("CreateReview"); 
            }
            else
            {
                return null;
            }
        }


        public IActionResult Index(int? type, int? cityid, DateTime? date)
        {
            string? jwtToken = Request.Cookies["eventhive"];
            var events = _context.Events.Where(e => e.DateTime.ToLocalTime() > DateTime.Now).OrderBy(e => e.DateTime).ToList();
            ViewBag.Title = "EventHive - Мероприятия";
            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Cities = [.. _context.Cities], Regions = [.. _context.Regions], Schools = [.. _context.Schools] };
                if (type == null || cityid == null || date == null)
                    ViewBag.Events = events;
                else
                {
                    if (type != null)
                    {
                        events = events.Where(q => q.EventType == ContextManager.ConvertIntToEvType((int)type)).ToList();
                        ViewBag.SelectedEventType = type;
                    }
                    if (cityid != null)
                    {
                        events = events.Where(e => _context.Schools.Any(s => s.Id == e.SchoolId && s.CityId == cityid)).ToList();
                        ViewBag.SelectedEventCityId = cityid;
                    }
                    if (date.HasValue)
                    {
                        events = events.Where(q => q.DateTime.Date == date.Value.Date).ToList();
                        ViewBag.SelectedEventDate = date;
                    }
                    ViewBag.Events = events;
                }

                if (model.CurrentUser.RoleId < 3)
                    return View(model);
                else
                    return Redirect("~/Administration/Index");
            }
            else
            {
                ViewBag.Events = events;
                var model = new ContextManager { Users = [.. _context.Users], Cities = [.. _context.Cities], Regions = [.. _context.Regions], Schools = [.. _context.Schools] };
                return View(model);
            }
        }

        public IActionResult School (int? schoolid)
        {
            if (schoolid == null)
            {
                return Redirect("~/Home/Error");
            }

            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                var model = new ContextManager{ CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Schools = [.. _context.Schools] };

                var school = _context.Schools.First(q => q.Id == schoolid);
                ViewBag.Events = _context.Events.Where(q => q.SchoolId == school.Id).ToList();
                ViewBag.School = school;

                ViewBag.Title = "EventHive - Записи на мероприятия";

                return View(model);
            }
            else
                return Redirect("~/Home/Error");
        }

        [HttpGet]
        public ActionResult EditActivity(int eventid)
        {
            string? jwtToken = Request.Cookies["eventhive"];

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                ViewBag.Event = _context.Events.First(q => q.Id == eventid);

                return PartialView("EditActivity");
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }
    }
}
