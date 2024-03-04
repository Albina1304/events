using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace events.Models
{
	public class Event
	{
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Дата и время")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Display(Name = "Вместимость")]
        [DataType(DataType.Text)]
        public int Capacity { get; set; }

        [ScaffoldColumn(false)]
        public bool? IsDeleted { get; set; }

        [ScaffoldColumn(false)]
        public bool? IsModified { get; set;}
        public string Photo { get; set; }
        public EventType EventType {  get; set; }
        public int SchoolId { get; set; }
    }

    public enum EventType
    {
        [Description("Спортивные соревнования")]
        Спортивные1соревнования,

        [Description("Культурные мероприятия")]
        Культурные1мероприятия,

        [Description("Конференции и семинары")]
        Конференции1и1семинары,

        [Description("Экскурсии")]
        Экскурсии,

        [Description("День открытых дверей")]
        День1открытых1дверей,

        [Description("Выпускные вечера")]
        Выпускные1вечера
    }
}
