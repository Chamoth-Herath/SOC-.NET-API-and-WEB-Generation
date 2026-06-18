using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web1.Pages
{
    public class KmcEventReadDTO
    {
        [JsonPropertyName("id")]                 public int      Id                  { get; set; }
        [JsonPropertyName("eventTitle")]         public string?  EventTitle          { get; set; }
        [JsonPropertyName("eventDescription")]   public string?  EventDescription    { get; set; }
        [JsonPropertyName("category")]           public string?  Category            { get; set; }
        [JsonPropertyName("venue")]              public string?  Venue               { get; set; }
        [JsonPropertyName("date")]               public DateTime Date                { get; set; }
        [JsonPropertyName("time")]               public DateTime Time                { get; set; }
        [JsonPropertyName("maxCapacity")]        public int      MaxCapacity         { get; set; }
        [JsonPropertyName("activityImage")]      public string?  ActivityImage       { get; set; }
        [JsonPropertyName("eventStatus")]        public string?  EventStatus         { get; set; }
        [JsonPropertyName("registeredCitizens")] public int      RegisteredCitizens  { get; set; }
        [JsonPropertyName("organizerId")]        public int      OrganizerId         { get; set; }
        [JsonPropertyName("organizerName")]      public string?  OrganizerName       { get; set; }
        [JsonPropertyName("filterPlatform")]     public string?  FilterPlatform      { get; set; }
    }

    public class KmcOrganizerDashboardModel : PageModel
    {
        private const string ServiceRoot = "https://localhost:7098";

        public List<KmcEventReadDTO> Events { get; set; } = new();
        public string? OrganizerName { get; set; }
        public int     OrganizerId   { get; set; }
        public string? OrganizerRole { get; set; }

        private HttpClient MakeClient()
        {
            var h = new HttpClientHandler();
            h.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            return new HttpClient(h);
        }

		public async Task<IActionResult> OnGetAsync()
		{
			OrganizerRole = HttpContext.Session.GetString("Role");
			OrganizerId = HttpContext.Session.GetInt32("UserId") ?? 0;
			OrganizerName = HttpContext.Session.GetString("FullName") ?? "Organizer";

			if (OrganizerRole != "Organizer")
				return RedirectToPage("/Index");

			var http = MakeClient();
			var res = await http.GetAsync($"{ServiceRoot}/api/Event/organizer/{OrganizerId}");
			if (res.IsSuccessStatusCode)
			{
				var json = await res.Content.ReadAsStringAsync();
				var all = JsonSerializer.Deserialize<List<KmcEventReadDTO>>(json,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
					?? new List<KmcEventReadDTO>();

				// Only show events created from KMC dashboard
				Events = all.Where(e => e.FilterPlatform == "KMC_WEB").ToList();
			}

			return Page();
		}

		// ── GET Registrations for an event ──
		public async Task<IActionResult> OnGetGetRegsAsync(int eventId)
        {
            var http = MakeClient();
            var res  = await http.GetAsync($"{ServiceRoot}/api/EventRegistration/event/{eventId}");
            if (!res.IsSuccessStatusCode)
                return new JsonResult(new List<object>());

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<JsonElement>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return new JsonResult(data);
        }

        // ── POST Delete ──
        public async Task<IActionResult> OnPostDeleteAsync(int eventId)
        {
            var http = MakeClient();
            var res  = await http.DeleteAsync($"{ServiceRoot}/api/Event/{eventId}");
            return res.IsSuccessStatusCode ? new OkResult() : BadRequest("Failed to delete.");
        }
    }
}