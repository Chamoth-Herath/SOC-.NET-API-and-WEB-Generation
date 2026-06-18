namespace SOC_API.DTO
{
	public class OrganizerWriteDTO
	{
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? OrganizationName { get; set; }
		public string? Password { get; set; }
	}

	public class OrganizerReadDTO
	{
		public int Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? OrganizationName { get; set; }
		public int TotalEvents { get; set; }        
	}
}