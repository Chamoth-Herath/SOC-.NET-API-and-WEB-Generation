using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web1.Pages
{
	[IgnoreAntiforgeryToken]
	public class SigninModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public void OnGet() { }

		public async Task<IActionResult> OnPostAsync(string typedEmail, string typedPassword, string selectedRole)
		{
			// Hardcoded organizer credentials — no API call needed
			if (selectedRole == "Organizer")
			{
				if (typedEmail == "chamothORG@gmail.com" && typedPassword == "123")
				{
					HttpContext.Session.SetString("Email", "chamothORG@gmail.com");
					HttpContext.Session.SetString("FullName", "Chamoth Organizer");
					HttpContext.Session.SetString("Role", "Organizer");
					HttpContext.Session.SetInt32("UserId", 2); 
					return new JsonResult(new { ok = true });
				}
				return new JsonResult(new { ok = false, message = "Invalid organizer credentials." });
			}

			// Citizen login via API
			string loginEndpoint = $"{ServiceRoot}/api/PublicCitizen/login?email={typedEmail}&password={typedPassword}";

			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient httpClient = new HttpClient(devHandler);
			HttpResponseMessage reply = await httpClient.PostAsync(loginEndpoint, null);

			if (!reply.IsSuccessStatusCode)
				return new JsonResult(new { ok = false, message = "Invalid email or password." });

			string rawJson = await reply.Content.ReadAsStringAsync();
			JsonDocument parsedDoc = JsonDocument.Parse(rawJson);
			JsonElement rootNode = parsedDoc.RootElement;

			HttpContext.Session.SetString("Email", rootNode.GetProperty("email").GetString() ?? typedEmail);
			HttpContext.Session.SetString("FullName", rootNode.GetProperty("fullName").GetString() ?? typedEmail);
			HttpContext.Session.SetString("Role", "Citizen");
			HttpContext.Session.SetInt32("UserId", rootNode.GetProperty("id").GetInt32());

			return new JsonResult(new { ok = true });
		}
	}
}