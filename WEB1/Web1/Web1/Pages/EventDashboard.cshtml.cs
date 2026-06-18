using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web1.Pages
{
	public class DashEventReadDTO
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

	[IgnoreAntiforgeryToken]
	public class EventDashboardModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public List<DashEventReadDTO> Events { get; set; } = new();
		public int OrganizerId { get; set; }

		// ── shared handler setup ──
		private HttpClient MakeClient()
		{
			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
			return new HttpClient(devHandler);
		}

		public async Task<IActionResult> OnGetAsync()
		{
			string? loggedInRole = HttpContext.Session.GetString("Role");
			OrganizerId = HttpContext.Session.GetInt32("UserId") ?? 0;

			if (loggedInRole != "Organizer" || OrganizerId == 0)
				return RedirectToPage("/Index");

			HttpClient httpClient = MakeClient();
			HttpResponseMessage eventsReply = await httpClient.GetAsync(
				$"{ServiceRoot}/api/Event/organizer/{OrganizerId}");

			if (eventsReply.IsSuccessStatusCode)
			{
				string rawJson = await eventsReply.Content.ReadAsStringAsync();
				JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				List<DashEventReadDTO>? fetched = JsonSerializer.Deserialize<List<DashEventReadDTO>>(rawJson, jsonOptions);
				if (fetched != null)
					Events = fetched.OrderByDescending(e => e.Date).ToList();
			}

			return Page();
		}

		// ── CREATE ──
		public async Task<IActionResult> OnPostCreateAsync()
		{
			OrganizerId = HttpContext.Session.GetInt32("UserId") ?? 0;
			if (OrganizerId == 0) return Unauthorized();

			HttpClient httpClient = MakeClient();
			MultipartFormDataContent forwardedForm = BuildForm();

			HttpResponseMessage createReply = await httpClient.PostAsync(
				$"{ServiceRoot}/api/Event", forwardedForm);

			if (!createReply.IsSuccessStatusCode)
			{
				string failReason = await createReply.Content.ReadAsStringAsync();
				return StatusCode((int)createReply.StatusCode, failReason);
			}

			return new JsonResult(new { ok = true });
		}

		// ── UPDATE ──
		public async Task<IActionResult> OnPostUpdateAsync([FromQuery] int eventId)
		{
			OrganizerId = HttpContext.Session.GetInt32("UserId") ?? 0;
			if (OrganizerId == 0) return Unauthorized();

			HttpClient httpClient = MakeClient();
			MultipartFormDataContent forwardedForm = BuildForm();

			HttpResponseMessage updateReply = await httpClient.PutAsync(
				$"{ServiceRoot}/api/Event/{eventId}", forwardedForm);

			if (!updateReply.IsSuccessStatusCode)
			{
				string failReason = await updateReply.Content.ReadAsStringAsync();
				return StatusCode((int)updateReply.StatusCode, failReason);
			}

			return new JsonResult(new { ok = true });
		}

		// ── DELETE ──
		public async Task<IActionResult> OnPostDeleteAsync()
		{
			string rawId = Request.Form["eventId"].ToString();
			bool parsed = int.TryParse(rawId, out int eventId);

			if (!parsed || eventId == 0)
				return BadRequest("Invalid event ID.");

			OrganizerId = HttpContext.Session.GetInt32("UserId") ?? 0;
			if (OrganizerId == 0) return Unauthorized();

			HttpClient httpClient = MakeClient();
			HttpResponseMessage deleteReply = await httpClient.DeleteAsync(
				$"{ServiceRoot}/api/Event/{eventId}");

			if (!deleteReply.IsSuccessStatusCode)
			{
				string failReason = await deleteReply.Content.ReadAsStringAsync();
				return StatusCode((int)deleteReply.StatusCode, failReason);
			}

			return new JsonResult(new { ok = true });
		}

		// ── GET REGISTRATIONS ──
		public async Task<IActionResult> OnGetGetRegsAsync(int eventId)
		{
			HttpClient httpClient = MakeClient();
			HttpResponseMessage regReply = await httpClient.GetAsync(
				$"{ServiceRoot}/api/EventRegistration/event/{eventId}");

			if (!regReply.IsSuccessStatusCode)
				return new JsonResult(new List<object>());

			string rawJson = await regReply.Content.ReadAsStringAsync();
			return Content(rawJson, "application/json");
		}

		// ── SHARED FORM BUILDER ──
		private MultipartFormDataContent BuildForm()
		{
			MultipartFormDataContent form = new MultipartFormDataContent();

			foreach (var field in new[]
			{
				"eventTitle","eventDescription","category","venue",
				"date","time","maxCapacity","eventStatus","organizerId","filterplatform"
			})
			{
				string val = Request.Form[field].ToString();
				if (!string.IsNullOrEmpty(val))
					form.Add(new StringContent(val), field);
			}

			if (Request.Form.Files.Count > 0)
			{
				IFormFile imgFile = Request.Form.Files[0];
				StreamContent imgContent = new StreamContent(imgFile.OpenReadStream());
				imgContent.Headers.ContentType =
					new System.Net.Http.Headers.MediaTypeHeaderValue(imgFile.ContentType);
				form.Add(imgContent, "activityImageFile", imgFile.FileName);
			}

			return form;
		}
	}
}