using events.Data;
using events.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademyController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AcademyController(ApiContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Получение всех пользоватей зарегистрированных на мероприятие - eventid
        /// </summary>
        /// <param name="eventid"></param>
        /// <returns></returns>
        [HttpGet("GetAllRegisteredUsersBySchoolEvent/{eventid}")]
        public JsonResult GetAllRegisteredUsersBySchoolEvent(int eventid)
        {
            var userevents = _context.UserEvents.Where(q => q.EventId == eventid).ToList();

            List<User> users = [];
            foreach (var user in userevents) 
            {
                var us = _context.Users.First(q => q.Id == user.UserId);
                users.Add(us);
            }

            return new JsonResult(Ok(users));
        }

        /// <summary>
        /// Добавление школьной фотографии
        /// </summary>
        /// <param name="schoolid"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost("AddSchoolPhoto/{schoolid}")]
        public async Task<IActionResult> AddSchoolPhoto(int schoolid, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return new JsonResult(BadRequest(500));
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var school = _context.Schools.SingleOrDefault(q => q.Id == schoolid);
                    if (school == null)
                    {
                        return new JsonResult(NotFound());
                    }

                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "schoolphoto");

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    school.Photo = uniqueFileName;
                    _context.SaveChanges();

                    return new JsonResult(Ok(school));
                }
            }
            catch
            {
                return new JsonResult(BadRequest(500));
            }
        }

        /// <summary>
        /// Обновление информации о школе
        /// </summary>
        /// <param name="schoolid"></param>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPut("UpdateInfo/{schoolid}/{name}/{desc}")]
        public IActionResult UpdateInfo(int schoolid, string name, string desc)
        {
            var school = _context.Schools.SingleOrDefault(q => q.Id == schoolid);

            if (school == null)
            {
                return new JsonResult(NotFound());
            }

            school.Name = name;
            school.Description = desc;

            _context.Schools.Update(school);
            _context.SaveChanges();

            return new JsonResult(Ok(school));
        }

        /// <summary>
        /// Обновление местополежения школы
        /// </summary>
        /// <param name="schoolid"></param>
        /// <param name="cityid"></param>
        /// <param name="street"></param>
        /// <param name="house"></param>
        /// <returns></returns>
        [HttpPut("UpdateLocation/{schoolid}/{cityid}/{street}/{house}")]
        public IActionResult UpdateLocation(int schoolid, int cityid, string street, string house)
        {
            var school = _context.Schools.SingleOrDefault(q => q.Id == schoolid);

            if (school == null)
                return new JsonResult(NotFound());

            school.CityId = cityid;
            school.Street = street;
            school.House = house;

            _context.Schools.Update(school);
            _context.SaveChanges();

            return new JsonResult(Ok(school));
        }

        /// <summary>
        /// Создание школы
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="desc"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost("Create/{name}/{email}/{desc}")]
        public async Task<IActionResult> Create(string name, string email, string desc, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return new JsonResult(BadRequest(500));
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "schoolphoto");

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    var userpass = ContextManager.GeneratePassword();

                    var newuser = new User()
                    {
                        Email = email,
                        Password = ContextManager.ComputeSha256Hash(userpass),
                        CreateionDate = DateTime.UtcNow,
                        RoleId = 2,
                        IsDeleted = false
                    };

                    _context.Users.Add(newuser);
                    _context.SaveChanges();

                    var school = new School()
                    {
                        Name = name,
                        Description = desc,
                        Photo = uniqueFileName,
                        UserId = newuser.Id,
                        IsDeleted = false
                    };

                    _context.Schools.Add(school);
                    _context.SaveChanges();

                    ContextManager.SendMessageToEmail(email, "Регстрация школы на сервисе EventHive", $"Здравствуйте, вы подали заявку на регистрацию вашей школы.\nДля доступа к системе используйте следующие данные:\nEmail: {email}\nPassword: {userpass}");

                    return new JsonResult(Ok(newuser));
                }
            }
            catch
            {
                return new JsonResult(BadRequest(500));
            }
        }

        [HttpPut("Block/{schoolid}")]
        public JsonResult Block(int schoolid)
        {
            try
            {
                var school = _context.Schools.First(q => q.Id == schoolid);

                school.IsDeleted = true;

                _context.Schools.Update(school);
                _context.SaveChanges();

                return new JsonResult(Ok(school));
            }
            catch
            {
                return new JsonResult(BadRequest());
            }
        }

        [HttpPut("Unblock/{schoolid}")]
        public JsonResult Unblock(int schoolid)
        {
            try
            {
                var school = _context.Schools.First(q => q.Id == schoolid);

                school.IsDeleted = false;

                _context.Schools.Update(school);
                _context.SaveChanges();

                return new JsonResult(Ok(school));
            }
            catch
            {
                return new JsonResult(BadRequest());
            }
        }
    }
}
