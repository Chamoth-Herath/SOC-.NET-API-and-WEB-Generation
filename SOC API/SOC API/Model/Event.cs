using System.ComponentModel.DataAnnotations;

namespace SOC_API.Model
{
	public class Event
	{
		[Key]
		public int Id { get; set; }

		public string? EventTitle { get; set; }

		public string? EventDescription { get; set; }

		public string? Category { get; set; }

		public string? Venue { get; set; }

		public DateTime Date { get; set; }
		public DateTime Time { get; set; }	

		public int MaxCapacity { get; set; }

		public string? ActivityImage { get; set; }

		public string? EventStatus { get; set; }

		public int RegisteredCitizens { get; set; } = 0;

		public int OrganizerId { get; set; }

		public string FilterPlatform { get; set; } = "PrivateWEB";
		public Organizer? Organizer { get; set; }

		public List<EventRegister> Registrations { get; set; } = new List<EventRegister>();
	}
}