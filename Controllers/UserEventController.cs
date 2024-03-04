using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using events.Models;
using events.Data;
using Microsoft.EntityFrameworkCore;

namespace events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventController : ControllerBase
    {
        private readonly ApiContext _context;
        public UserEventController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Регистрация на мероприятие
        /// </summary>
        /// <param name="iduser"></param>
        /// <param name="idevent"></param>
        /// <returns></returns>
        [HttpPost("EventRegistration/{iduser}/{idevent}")]
        public JsonResult EventRegistration(int iduser, int idevent)
        {
            try
            {
                if (iduser != 0 && idevent != 0)
                {
                    UserEvent userEvent = new UserEvent()
                    {
                        UserId = iduser,
                        EventId = idevent
                    };
                    _context.Add(userEvent);
                    _context.SaveChanges();

                    var ev = _context.Events.Single(q => q.Id == idevent);
                    ev.Capacity -= 1;
                    _context.Events.Update(ev);
                    _context.SaveChanges();

                    var user = _context.Users.Single(x => x.Id == iduser);
                    var school = _context.Schools.Single(x => x.Id == ev.SchoolId);

                    string evaddress = $"{_context.Regions.Single(q => q.Id == _context.Cities.Single(x => x.Id == school.CityId).RegionId).Name}, город {_context.Cities.Single(x => x.Id == school.CityId).Name}, улица {school.Street}, дом {school.House}";

                    ContextManager.SendMessageToEmail(user.Email, "Запись на мероприятие", $"Здравствуйте <b>{user.LastName} {user.FirstName} {user.MiddleName}</b>, вы записались на мероприятие {ev.Name}.<br>Дата и время мероприятия: <b>{ev.DateTime.AddHours(3).ToUniversalTime().ToString("dddd, d MMMM, HH:mm")}</b>.<br><br>Место проведения: <b>{evaddress}</b>.");

                    return new JsonResult(Ok(userEvent));
                }
                return new JsonResult(BadRequest());
            }
            catch
            {
                return new JsonResult(BadRequest(500));
            }
        }
    }
}
