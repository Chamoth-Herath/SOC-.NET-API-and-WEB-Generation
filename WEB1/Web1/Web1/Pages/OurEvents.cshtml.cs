using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web1.Pages
{
	public class EventReadDTO
	{
		[JsonPropertyName("id")] public int Id { get; set; }
		[JsonPropertyName("eventTitle")] public string? EventTitle { get; set; }
		[JsonPropertyName("eventDescription")] public string? EventDescription { get; set; }
		[JsonPropertyName("category")] public string? Category { get; set; }
		[JsonPropertyName("venue")] public string? Venue { get; set; }
		[JsonPropertyName("date")] public DateTime Date { get; set; }
		[JsonPropertyName("time")] public DateTime Time { get; set; }
		[JsonPropertyName("maxCapacity")] public int MaxCapacity { get; set; }
		[JsonPropertyName("activityImage")] public string? ActivityImage { get; set; }
		[JsonPropertyName("eventStatus")] public string? EventStatus { get; set; }
		[JsonPropertyName("registeredCitizens")] public int RegisteredCitizens { get; set; }
		[JsonPropertyName("organizerId")] public int OrganizerId { get; set; }
		[JsonPropertyName("organizerName")] public string? OrganizerName { get; set; }
	}

	public class ParticipantRegDTO
	{
		[JsonPropertyName("eventId")] public int EventId { get; set; }
	}

	public class OurEventsModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public List<EventReadDTO> Events { get; set; } = new();
		public List<string> Categories { get; set; } = new();
		public string? LoggedInRole { get; set; }
		public HashSet<int> RegisteredEventIds { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			LoggedInRole = HttpContext.Session.GetString("Role");
			int citizenId = HttpContext.Session.GetInt32("UserId") ?? 0;

			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient httpClient = new HttpClient(devHandler);

			JsonSerializerOptions jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			HttpResponseMessage eventsReply = await httpClient.GetAsync(
				$"{ServiceRoot}/api/Event");

			if (eventsReply.IsSuccessStatusCode)
			{
				string rawJson = await eventsReply.Content.ReadAsStringAsync();
				List<EventReadDTO>? fetched = JsonSerializer.Deserialize<List<EventReadDTO>>(rawJson, jsonOptions);

				if (fetched != null)
				{
					Events = fetched;
					Categories = fetched
						.Where(e => !string.IsNullOrEmpty(e.Category))
						.Select(e => e.Category!)
						.Distinct()
						.OrderBy(c => c)
						.ToList();
				}
			}

			
			if (LoggedInRole == "Citizen" && citizenId > 0)
			{
				HttpResponseMessage regReply = await httpClient.GetAsync(
					$"{ServiceRoot}/api/EventRegistration/participant/{citizenId}");

				if (regReply.IsSuccessStatusCode)
				{
					string regJson = await regReply.Content.ReadAsStringAsync();
					List<ParticipantRegDTO>? regs = JsonSerializer.Deserialize<List<ParticipantRegDTO>>(regJson, jsonOptions);

					if (regs != null)
						RegisteredEventIds = regs.Select(r => r.EventId).ToHashSet();
				}
			}

			return Page();
		}
	}
}