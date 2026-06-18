namespace SOC_API.DTO
{
	public class PublicCitizenWriteDTO
	{
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Password { get; set; }
	}

	public class PublicCitizenReadDTO
	{
		public int Id { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public int TotalRegistrations { get; set; }    
	}
}