using Microsoft.EntityFrameworkCore;
using SOC_API.Model;

namespace SOC_API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{ }

		
		public DbSet<Event> Events { get; set; }
		public DbSet<Organizer> Organizers { get; set; }
		public DbSet<PublicCitizen> PublicCitizens { get; set; }
		public DbSet<EventRegister> EventRegisters { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Event>()
				.HasOne(e => e.Organizer)
				.WithMany(o => o.Events)
				.HasForeignKey(e => e.OrganizerId)
				.OnDelete(DeleteBehavior.Restrict);   

			
			modelBuilder.Entity<EventRegister>()
				.HasOne(r => r.Event)
				.WithMany(e => e.Registrations)
				.HasForeignKey(r => r.EventId)
				.OnDelete(DeleteBehavior.Cascade);    

			
			modelBuilder.Entity<EventRegister>()
				.HasOne(r => r.PublicCitizen)
				.WithMany(p => p.Registrations)
				.HasForeignKey(r => r.PublicCitizenId)
				.OnDelete(DeleteBehavior.Cascade);

			
			modelBuilder.Entity<Event>()
				.Property(e => e.ActivityImage)
				.HasColumnType("nvarchar(max)");

			modelBuilder.Entity<Organizer>().HasData(new Organizer
			{
				Id = 2,
				FullName = "Chamoth",
				Email = "chamothORG@gmail.com",
				Password = "123",
				OrganizationName = "Chamoth Event Organizers"
			});


		}
	}
}