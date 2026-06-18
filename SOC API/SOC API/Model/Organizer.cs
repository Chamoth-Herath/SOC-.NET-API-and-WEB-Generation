using System.ComponentModel.DataAnnotations;

namespace SOC_API.Model
{
	public class Organizer
	{
		[Key]
		public int Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? OrganizationName { get; set; }
		public string? Password { get; set; }
		

		public List<Event> Events { get; set; } = new List<Event>();
	}
}