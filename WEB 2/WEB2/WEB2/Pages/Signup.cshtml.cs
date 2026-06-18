using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WEB2.Pages
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
			string registerEndpoint = selectedRole == "Organizer"
				? $"{ServiceRoot}/api/Organizer/register"
				: $"{ServiceRoot}/api/PublicCitizen/register";

			object fieldMap = selectedRole == "Organizer"
				? new { fullName = enteredName, email = enteredEmail, password = enteredPassword, organizationName = enteredOrg }
				: new { fullName = enteredName, email = enteredEmail, password = enteredPassword, phone = enteredPhone };

			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
			var http = new HttpClient(handler);

			var content = new StringContent(
				JsonSerializer.Serialize(fieldMap),
				System.Text.Encoding.UTF8,
				"application/json");

			var outcome = await http.PostAsync(registerEndpoint, content);
			if (!outcome.IsSuccessStatusCode)
			{
				var fail = await outcome.Content.ReadAsStringAsync();
				return new JsonResult(new
				{
					ok = false,
					message = string.IsNullOrWhiteSpace(fail) ? "Registration failed." : fail
				});
			}

			string loginEndpoint = selectedRole == "Organizer"
				? $"{ServiceRoot}/api/Organizer/login?email={enteredEmail}&password={enteredPassword}"
				: $"{ServiceRoot}/api/PublicCitizen/login?email={enteredEmail}&password={enteredPassword}";

			var loginHandler = new HttpClientHandler();
			loginHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
			var loginClient = new HttpClient(loginHandler);

			var loginReply = await loginClient.PostAsync(loginEndpoint, null);
			if (!loginReply.IsSuccessStatusCode)
				return new JsonResult(new { ok = false, message = "Registered but auto sign-in failed." });

			var loginRoot = JsonDocument.Parse(
				await loginReply.Content.ReadAsStringAsync()).RootElement;

			HttpContext.Session.SetString("Email", loginRoot.GetProperty("email").GetString() ?? enteredEmail);
			HttpContext.Session.SetString("FullName", loginRoot.GetProperty("fullName").GetString() ?? enteredEmail);
			HttpContext.Session.SetString("Role", selectedRole);
			HttpContext.Session.SetInt32("UserId", loginRoot.GetProperty("id").GetInt32());

			return new JsonResult(new { ok = true });
		}
	}
}