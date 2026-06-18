namespace SOC_API.DTO
{
	public class EventWriteDTO
	{
		public string? EventTitle { get; set; }
		public string? EventDescription { get; set; }
		public string? Category { get; set; }
		public string? Venue { get; set; }
		public DateTime Date { get; set; }
		public DateTime Time { get; set; }
		public int MaxCapacity { get; set; }
		public string? ActivityImage { get; set; }
		public string? EventStatus { get; set; }
		public int OrganizerId { get; set; }
	}

	public class EventReadDTO
	{
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
		public string FilterPlatform { get; set; }
		public int RegisteredCitizens { get; set; }
		public int OrganizerId { get; set; }
		public string? OrganizerName { get; set; }   
	}
}