using events.Data;
using events.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace events.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiContext _context;
        public HomeController(ApiContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string? jwtToken = Request.Cookies["eventhive"];

            var upcomingEvents = _context.Events.Where(e => e.DateTime.ToLocalTime() > DateTime.Now).OrderBy(e => e.DateTime).Take(6).ToList();
            if (upcomingEvents.Count() > 0)
                ViewBag.Events = upcomingEvents;
            else
                ViewBag.Events = null;
            ViewBag.Title = "EventHive";

            var schools = _context.Schools.ToList();
            var userevents = _context.UserEvents.ToList();

            var popularEvents = _context.UserEvents
                .GroupBy(ue => ue.EventId) // Группируем записи UserEvent по EventId
                .Select(g => new { EventId = g.Key, Count = g.Count() })  // Вычисляем количество записей для каждого EventId
                .OrderByDescending(e => e.Count)  // Сортируем по количеству записей, начиная с самого популярного
                .Select(e => new { Count = e.Count, Event = _context.Events.FirstOrDefault(ev => ev.Id == e.EventId) })
                .ToList();

            var popularSchools = popularEvents
                .Select(pe => new { Event = pe.Event, Count = pe.Count })
                .GroupBy(e => e.Event.SchoolId) // Группируем события по SchoolId
                .Select(g => new { SchoolId = g.Key, School = _context.Schools.FirstOrDefault(s => s.Id == g.Key), Count = g.Sum(i => i.Count) })
                .OrderByDescending(s => s.Count)
                .ToList();

            ViewBag.PopularEvents = popularEvents;
            ViewBag.PopularSchools = popularSchools;

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;

                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Users = [.. _context.Users], Cities = [.. _context.Cities], Regions = [.. _context.Regions], Schools = [.. _context.Schools] };
                ViewBag.User = model.CurrentUser;

                if (model.CurrentUser.RoleId < 3)
                    return View(model);
                else 
                    return Redirect("~/Administration/Index");
            }
            else
            {
                var model = new ContextManager { Users = [.. _context.Users], Cities = [.. _context.Cities], Regions = [.. _context.Regions], Schools = [.. _context.Schools] };
                return View(model);
            }
        }

        public IActionResult Error()
        {
            string? jwtToken = Request.Cookies["eventhive"];
            ViewBag.Title = "EventHive - Ошибка";

            if (!string.IsNullOrEmpty(jwtToken) && ContextManager.IsJwtTokenValid(jwtToken))
            {
                var jwt = Request.Cookies["eventhive"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var userId = token.Claims.First(c => c.Type == "userId").Value;
                
                var model = new ContextManager { CurrentUser = _context.Users.SingleOrDefault(m => m.Id == int.Parse(userId)), Schools = [.. _context.Schools] };
                return View(model); // Возвращает представление для отображения ошибки
            }
            else
                return View(new ContextManager());
                
        }
    }

    public class SchoolPopularity
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int UserEventCount { get; set; }
    }
}
