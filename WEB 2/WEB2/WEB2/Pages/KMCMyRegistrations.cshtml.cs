using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WEB2.Pages
{
	public class MyRegDTO
	{
		[JsonPropertyName("id")] public int Id { get; set; }
		[JsonPropertyName("eventId")] public int EventId { get; set; }
		[JsonPropertyName("registeredAt")] public DateTime RegisteredAt { get; set; }
		[JsonPropertyName("eventTitle")] public string? EventTitle { get; set; }
		[JsonPropertyName("venue")] public string? Venue { get; set; }
		[JsonPropertyName("date")] public DateTime Date { get; set; }
		[JsonPropertyName("time")] public DateTime Time { get; set; }
		[JsonPropertyName("activityImage")] public string? ActivityImage { get; set; }
		[JsonPropertyName("category")] public string? Category { get; set; }
	}

	public class MyRegistrationsModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public List<MyRegDTO> Registrations { get; set; } = new();
		public string? CitizenName { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			string? loggedInRole = HttpContext.Session.GetString("Role");
			int citizenId = HttpContext.Session.GetInt32("UserId") ?? 0;
			CitizenName = HttpContext.Session.GetString("Email");

			if (loggedInRole != "Citizen" || citizenId == 0)
				return RedirectToPage("/Index");

			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient httpClient = new HttpClient(devHandler);
			HttpResponseMessage regReply = await httpClient.GetAsync(
				$"{ServiceRoot}/api/EventRegistration/participant/{citizenId}");

			if (regReply.IsSuccessStatusCode)
			{
				string rawJson = await regReply.Content.ReadAsStringAsync();

				JsonSerializerOptions jsonOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				};

				List<MyRegDTO>? fetched = JsonSerializer.Deserialize<List<MyRegDTO>>(rawJson, jsonOptions);

				if (fetched != null)
					Registrations = fetched.OrderBy(r => r.Date).ToList();
			}

			return Page();
		}

		public string GetPillClass(DateTime eventDate)
		{
			int daysLeft = (eventDate.Date - DateTime.Today).Days;
			if (daysLeft < 0) return "days-past";
			if (daysLeft == 0) return "days-today";
			if (daysLeft <= 3) return "days-urgent";
			if (daysLeft <= 7) return "days-soon";
			return "days-far";
		}

		public string GetDaysText(DateTime eventDate)
		{
			int daysLeft = (eventDate.Date - DateTime.Today).Days;
			if (daysLeft < 0) return "Past";
			if (daysLeft == 0) return "Today!";
			if (daysLeft == 1) return "1 day";
			return $"{daysLeft} days";
		}

		public string GetDaysLabel(DateTime eventDate)
		{
			int daysLeft = (eventDate.Date - DateTime.Today).Days;
			return daysLeft <= 0 ? "" : "left";
		}
	}
}