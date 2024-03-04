using events.Models;
using Microsoft.EntityFrameworkCore;

namespace events.Data
{
	public class ApiContext : DbContext
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserEvent> UserEvents { get; set; }
		public DbSet<School> Schools { get; set; }
        public DbSet<EventReview> EventReviews { get; set; }
		public DbSet<Region> Regions { get; set; }
		public DbSet<City> Cities { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
	}
}
