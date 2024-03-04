using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using events.Models;
using events.Data;
using Microsoft.EntityFrameworkCore;

namespace events.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventsController : ControllerBase
	{
		private readonly ApiContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EventsController(ApiContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		/// <summary>
		/// Обновление мероприятия
		/// </summary>
		/// <param name="eventid"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="datetime"></param>
		/// <param name="eventtype"></param>
		/// <returns></returns>
		[HttpPut("Update/{eventid}/{name}/{description}/{datetime}/{eventtype}")]
		public JsonResult Update(int eventid, string name, string description, DateTime datetime, int eventtype)
		{
			try
			{
				var ev = _context.Events.First(q => q.Id == eventid);

				ev.Name = name;
				ev.Description = description;
				ev.DateTime = datetime.ToUniversalTime();
				ev.EventType = ContextManager.ConvertIntToEvType(eventtype);
				ev.IsModified = true;

				_context.Events.Update(ev);
				_context.SaveChanges();

                return new JsonResult(Ok(true));
            }
			catch
			{
				return new JsonResult(Ok(false));
			}
		}

        /// <summary>
		/// Создание мероприятия
		/// </summary>
		/// <param name="schoolid"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="dateTime"></param>
		/// <param name="capacity"></param>
		/// <param name="eventtype"></param>
		/// <param name="photo"></param>
		/// <returns></returns>
        [HttpPost("Create/{schoolid}/{name}/{description}/{dateTime}/{capacity}/{eventtype}")]
		public async Task<IActionResult> Create(Event even, IFormFile photo)
		{
            if (photo == null || photo.Length == 0)
            {
                return new JsonResult(BadRequest(500));
            }

			using (var memoryStream = new MemoryStream())
			{
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "eventphoto");

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

				var newevent = new Event()
				{
					Name = name,
					Description = description,
					DateTime = dateTime,
					Capacity = capacity,
					IsDeleted = false,
					IsModified = false,
					Photo = uniqueFileName,
					SchoolId = schoolid,
					EventType = ContextManager.ConvertIntToEvType(eventtype)
                };

				_context.Events.Add(newevent);
				_context.SaveChanges();
                return new JsonResult(Ok(newevent));
            }
		}

		/// <summary>
		/// Блокировка мероприятия
		/// </summary>
		/// <param name="eventid"></param>
		/// <returns></returns>
		[HttpPost("Block/{eventid}")]
		public JsonResult Block (int eventid)
		{
            var eventt = _context.Events.SingleOrDefault(q => q.Id == eventid);

			if (eventt != null)
			{
				eventt.IsDeleted = true;
				_context.Events.Update(eventt);
				_context.SaveChanges();
			}
			else
				return new JsonResult(BadRequest());

			return new JsonResult(Ok(eventt));
        }

		/// <summary>
		/// Разблокировка мероприятия
		/// </summary>
		/// <param name="eventid"></param>
		/// <returns></returns>
        [HttpPost("Unblock/{eventid}")]
        public JsonResult Unblock(int eventid)
        {
            var eventt = _context.Events.SingleOrDefault(q => q.Id == eventid);

            if (eventt != null)
            {
                eventt.IsDeleted = false;
                _context.Events.Update(eventt);
                _context.SaveChanges();
            }
            else
                return new JsonResult(BadRequest());

            return new JsonResult(Ok(eventt));
        }
    }
}
