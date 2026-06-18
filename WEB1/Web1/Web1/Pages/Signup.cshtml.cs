using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web1.Pages
{
	[IgnoreAntiforgeryToken]
	public class SignupModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";

		public void OnGet() { }

		public async Task<IActionResult> OnPostAsync(
	string enteredName,
	string enteredEmail,
	string enteredPassword,
	string enteredPhone,
	string enteredOrg,
	string selectedRole)
		{
			// Organizers cannot self-register
			if (selectedRole == "Organizer")
				return new JsonResult(new { ok = false, message = "Organizer registration is not allowed." });

			string registerEndpoint = $"{ServiceRoot}/api/PublicCitizen/register";

			object fieldMap = new { fullName = enteredName, email = enteredEmail, password = enteredPassword, phone = enteredPhone };

			HttpClientHandler devHandler = new HttpClientHandler();
			devHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient httpClient = new HttpClient(devHandler);
			string requestBody = JsonSerializer.Serialize(fieldMap);
			StringContent packedContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

			HttpResponseMessage outcome = await httpClient.PostAsync(registerEndpoint, packedContent);

			if (!outcome.IsSuccessStatusCode)
			{
				string failReason = await outcome.Content.ReadAsStringAsync();
				return new JsonResult(new { ok = false, message = string.IsNullOrWhiteSpace(failReason) ? "Registration failed." : failReason });
			}

			// Auto sign-in after registration
			string loginEndpoint = $"{ServiceRoot}/api/PublicCitizen/login?email={enteredEmail}&password={enteredPassword}";

			HttpClientHandler loginHandler = new HttpClientHandler();
			loginHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			HttpClient loginClient = new HttpClient(loginHandler);
			HttpResponseMessage loginReply = await loginClient.PostAsync(loginEndpoint, null);

			if (!loginReply.IsSuccessStatusCode)
				return new JsonResult(new { ok = false, message = "Registered but auto sign-in failed." });

			string loginRaw = await loginReply.Content.ReadAsStringAsync();
			JsonDocument loginDoc = JsonDocument.Parse(loginRaw);
			JsonElement loginRoot = loginDoc.RootElement;

			HttpContext.Session.SetString("Email", loginRoot.GetProperty("email").GetString() ?? enteredEmail);
			HttpContext.Session.SetString("FullName", loginRoot.GetProperty("fullName").GetString() ?? enteredEmail);
			HttpContext.Session.SetString("Role", "Citizen");
			HttpContext.Session.SetInt32("UserId", loginRoot.GetProperty("id").GetInt32());

			return new JsonResult(new { ok = true });
		}
	}
}