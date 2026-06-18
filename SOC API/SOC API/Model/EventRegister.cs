using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOC_API.Model
{
	public class EventRegister
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public DateTime RegisteredAt { get; set; } = DateTime.Now;

		public int EventId { get; set; }

		public Event Event { get; set; }
		public int PublicCitizenId { get; set; }
		public PublicCitizen? PublicCitizen { get; set; }
	}
}
