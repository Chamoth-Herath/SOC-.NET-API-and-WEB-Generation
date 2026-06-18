namespace SOC_API.DTO
{
	public class EventRegisterWriteDTO
	{
		public int EventId { get; set; }
		public int PublicCitizenId { get; set; }
	}

	public class EventRegisterReadDTO
	{
		public int Id { get; set; }
		public DateTime RegisteredAt { get; set; }
		public DateTime Date { get; set; }
		public int EventId { get; set; }
		public string? PublicCitizenEmail { get; set; }
		public string? EventTitle { get; set; } 
		public string? Venue { get; set; }
		public string? PublicCitizenName { get; set; }      
	}
}