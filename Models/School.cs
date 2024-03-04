namespace events.Models
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public int? CityId { get; set; }
        public string? Street { get; set; }
        public string? House { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
    }
}
