using events.Data;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace events.Controllers
{
    public class SchoolsController : Controller
    {
        private readonly ApiContext _context;
        public SchoolsController(ApiContext context) 
        {
            _context = context;
        }
        public IActionResult School(int schoolid)
        {
            string? jwtToken = Request.Cookies["eventhive"];

            var school = _context.Schools.Single(q => q.Id == schoolid);
            ViewBag.School = school;
            ViewBag.Title = $"EventHive - {school.Name}";

            ViewBag.SchoolUpcomingEvents = _context.Events.Where(q => q.SchoolId == schoolid && q.DateTime.ToLocalTime() < DateTime.Now.ToLocalTime()).ToArray();
            ViewBag.SchoolPreviousEvents = _context.Events.Where(q => q.SchoolId == schoolid && q.DateTime.ToLocalTime() > DateTime.Now.ToLocalTime()).ToArray();
            ViewBag.SchoolAddress = _context.Regions.Single(q => q.Id == _context.Cities.Single(q => q.Id == school.CityId).RegionId).Name + ", " + _context.Cities.Single(q => q.Id == school.CityId).Name + ", " + school.Street + ", " + school.House;

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Schools = [.. _context.Schools] };

                if (model.CurrentUser.RoleId < 3)
                    return View(model);
                else
                    return Redirect("~/Administration/Index");
            }
            else
            {
                return View(new ContextManager());
            }
        }
    }
}
