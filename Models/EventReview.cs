namespace events.Models
{
    public class EventReview
    {
        public int Id { get; set; }
        public int Evaluation { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
    }
}
