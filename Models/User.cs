using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace events.Models
{
    public class User
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Электронная почта")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Имя")]
        [DataType(DataType.Text)]
        public string? FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [DataType(DataType.Text)]
        public string? LastName { get; set; }

        [Display(Name = "Отчество")]
        [DataType(DataType.Text)]
        public string? MiddleName { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime CreateionDate { get; set; }

        [ScaffoldColumn(false)]
        public int RoleId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; }
    }
}
