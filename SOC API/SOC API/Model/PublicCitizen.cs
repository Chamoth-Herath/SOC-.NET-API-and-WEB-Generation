using System.ComponentModel.DataAnnotations;

namespace SOC_API.Model
{
	public class PublicCitizen
	{
		[Key]
		public int Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Password { get; set; }

		public List<EventRegister> Registrations { get; set; } = new List<EventRegister>();
	}
}