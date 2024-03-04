using events.Data;
using events.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(ApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("CheckAvailableEmail/{email}")]
        public JsonResult CheckAvailableEmail(string email)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            if (user != null)
                return new JsonResult(Ok(false));
            else
                return new JsonResult(Ok(true));
        }

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost("Block/{userid}")]
        public JsonResult Block(int userid)
        {
            var user = _context.Users.SingleOrDefault(q => q.Id == userid);

            if (user != null)
            {
                user.IsDeleted = true;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            else
                return new JsonResult(NotFound());

            return new JsonResult(Ok(user));
        }

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost("Unblock/{userid}")]
        public JsonResult Unblock(int userid)
        {
            var user = _context.Users.SingleOrDefault(q => q.Id == userid);

            if (user != null)
            {
                user.IsDeleted = false;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            else
                return new JsonResult(NotFound());

            return new JsonResult(Ok(user));
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public JsonResult Create(User user)
        {
            if (user.Id == 0 && user.RoleId != 0)
            {
                string previouspass = user.Password;
                user.Password = ContextManager.ComputeSha256Hash(previouspass);
                _context.Users.Add(user);
            }
            else
                return new JsonResult(NotFound());

            _context.SaveChanges();
            return new JsonResult(Ok(user));
        }

        /// <summary>
        /// Обновление персональных данных
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="lastname"></param>
        /// <param name="firstname"></param>
        /// <param name="middlename"></param>
        /// <returns></returns>
        [HttpPut("UpdatePersonal/{userid}/{lastname}/{firstname}/{middlename}")]
        public JsonResult UpdatePersonal(int userid, string lastname, string firstname, string middlename)
        {
            var user = _context.Users.SingleOrDefault(q => q.Id == userid);
            if (user == null)
            {
                return new JsonResult(BadRequest());
            }

            user.LastName = lastname;
            user.FirstName = firstname;
            user.MiddleName = middlename;

            _context.Users.Update(user);
            _context.SaveChanges();

            return new JsonResult(Ok(user));
        }

        
        /// <summary>
        /// Обновление входных данных
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPut("UpdateLogon/{userid}/{password}")]
        public JsonResult UpdateLogon(int userid, string password)
        {
            var user = _context.Users.SingleOrDefault(q => q.Id == userid);
            if (user == null)
            {
                return new JsonResult(BadRequest());
            }

            user.Password = ContextManager.ComputeSha256Hash(password);

            _context.Users.Update(user);
            _context.SaveChanges();

            return new JsonResult(Ok(user));
        }

        /// <summary>
        /// Получение пользователя по почте и паролю
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet("GetByEmailAndPassword/{email}/{password}")]
        public JsonResult GetByEmailAndPassword(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(q => q.Email == email && q.Password == ContextManager.ComputeSha256Hash(password));
            if (user != null)
            {
                var jwtConfig = _configuration.GetSection("JwtConfig").Get<JwtConfig>();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("userId", user.Id.ToString()),
                        new Claim("email", user.Email),
                        new Claim("role", user.RoleId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes),
                    NotBefore = DateTime.UtcNow,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                Response.Cookies.Append(jwtConfig.CookieName, tokenHandler.WriteToken(token), new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes)
                });

                return new JsonResult(Ok(user));
            }
            else
                return new JsonResult(BadRequest());
        }

        [HttpPost("SendCodeByEmail/{email}")]
        public JsonResult SendCodeByEmail(string email)
        {
            int gencode = ContextManager.GenerateRecoveryCode();
            ContextManager.SendMessageToEmail(email, "EventHive - Восстановление пароля", $"Здравствуйте, вы запросили код для восстановления пароля.<br>Если это делали не вы, то проигнорируйте это письмо.<br><br>Код востановления пароля: <b>{gencode}</b>.");
            return new JsonResult(Ok());
        }

        [HttpPost("UpdatePasswordByEmailAndCode/{email}/{code}/{pass}")]
        public JsonResult UpdatePasswordByEmailAndCode(string email, int code, string pass)
        {
            if (code == ContextManager.code)
            {
                var user = _context.Users.First(q => q.Email == email);
                user.Password = ContextManager.ComputeSha256Hash(pass);
                _context.Users.Update(user);
                _context.SaveChanges();
                return new JsonResult(Ok(true));
            }
            else
            {
                return new JsonResult(Ok(false));
            }
        }
    }
}
