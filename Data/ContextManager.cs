using events.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace events.Data
{
    public class ContextManager
    {
        public User? CurrentUser { get; set; }
        public List<User>? Users { get; set; }
        public List<City>? Cities { get; set; }
        public List<Region>? Regions { get; set; }
        public List<UserEvent>? UserEvents { get; set; }
        public List<EventReview>? EventReviews { get; set; }
        public List<School>? Schools { get; set; }
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsJwtTokenValid(string jwtToken)
        {
            return !string.IsNullOrEmpty(jwtToken);
        }

        public static EventType ConvertIntToEvType (int evtype)
        {
            switch (evtype)
            {
                case 0:
                    return EventType.Спортивные1соревнования;
                case 1:
                    return EventType.Культурные1мероприятия;
                case 2:
                    return EventType.Конференции1и1семинары;
                case 3:
                    return EventType.Экскурсии;
                case 4:
                    return EventType.День1открытых1дверей;
                case 5:
                    return EventType.Выпускные1вечера;
                default:
                    return EventType.Культурные1мероприятия;
            }
        }
        public static string GetSchoolNameViaId(ApiContext _context, int schoolid)
        {
            return _context.Schools.Single(q => q.Id == schoolid).Name;
        }

        public string ConvertEvTypeToStr(EventType evtype)
        {
            switch (evtype)
            {
                case EventType.Спортивные1соревнования:
                    return "Спортивные соревнования";
                case EventType.Культурные1мероприятия:
                    return "Культурные мероприятия";
                case EventType.Конференции1и1семинары:
                    return "Конференции и семинары";
                case EventType.Экскурсии:
                    return "Экскурсии";
                case EventType.День1открытых1дверей:
                    return "День открытых дверей";
                case EventType.Выпускные1вечера:
                    return "Выпускные вечера";
                default:
                    return "Тип не выбран!";
            }
        }

        public static string GeneratePassword()
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 8; i++)
            {
                int index = random.Next(chars.Length);
                password.Append(chars[index]);
            }

            return password.ToString();
        }

        public async static void SendMessageToEmail(string email, string subject, string body)
        {
            MailAddress from = new MailAddress("eventhive@mail.ru", "EventHive");
            MailAddress to = new MailAddress(email);

            string htmlBody = $@"
            <div style='font-family: Arial, Helvetica, sans-serif;'>
                <div style='text-align:center;'>
                    <h1>Event<span style='color: #7848F4'>Hive</span></h1>
                </div>
                <div>
                    <p style='font-size: 16px; color: #000000 !important'>
                        {body}
                    </p>
                </div>
                <div style='text-align: center; background-color: rgb(16, 16, 123); padding: 16px 32px;'>
                    <p style='color: white; font-size: 20px;'>2024 &#8212; EventHive</p>
                </div>
            </div>";

            MailMessage message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential("eventhive@mail.ru", "rYg3pQwqe1FJJGmajneJ");
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(message);
        }

        public static int code = 0;
        public static int GenerateRecoveryCode()
        {
            Random rnd = new Random();
            code = rnd.Next(1000, 9999);
            return code;
        }
    }
}
