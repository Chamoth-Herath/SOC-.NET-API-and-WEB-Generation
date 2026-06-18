using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web1.Pages
{
	[IgnoreAntiforgeryToken]
	public class RegisterModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public async Task<IActionResult> OnPostAsync([FromBody] RegisterRequest requestBody)
		{
			int citizenId = HttpContext.Session.GetInt32("UserId") ?? 0;

			if (citizenId == 0)
				return new JsonResult(new { ok = false, message = "Not logged in." });

			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient httpClient = new HttpClient(devHandler);

			var payload = new
			{
				eventId = requestBody.EventId,
				publicCitizenId = citizenId
			};

			string requestJson = JsonSerializer.Serialize(payload);
			StringContent packedContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

			HttpResponseMessage registerReply = await httpClient.PostAsync(
				$"{ServiceRoot}/api/EventRegistration", packedContent);

			if (!registerReply.IsSuccessStatusCode)
			{
				string failReason = await registerReply.Content.ReadAsStringAsync();
				return new JsonResult(new { ok = false, message = failReason });
			}

			return new JsonResult(new { ok = true });
		}
	}

	public class RegisterRequest
	{
		public int EventId { get; set; }
	}
}