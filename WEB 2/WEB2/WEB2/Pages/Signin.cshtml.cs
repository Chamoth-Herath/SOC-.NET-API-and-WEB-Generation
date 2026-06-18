using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WEB2.Pages
{
	[IgnoreAntiforgeryToken]
	public class SigninModel : PageModel
	{
		private const string ServiceRoot = "https://localhost:7098";
		public void OnGet() { }

		public IActionResult OnGetLogout()
		{
			HttpContext.Session.Clear();
			return RedirectToPage("/Index");
		}

		public async Task<IActionResult> OnPostAsync(
	string typedEmail, string typedPassword, string selectedRole)
		{
			// Block KMC-only organizer from logging into this site
			if (selectedRole == "Organizer" &&
				string.Equals(typedEmail, "chamothORG@gmail.com", StringComparison.OrdinalIgnoreCase))
			{
				return new JsonResult(new { ok = false, message = "Invalid email or password." });
			}

			string loginEndpoint = selectedRole == "Organizer"
				? $"{ServiceRoot}/api/Organizer/login?email={typedEmail}&password={typedPassword}"
				: $"{ServiceRoot}/api/PublicCitizen/login?email={typedEmail}&password={typedPassword}";

			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
			var http = new HttpClient(handler);
			var reply = await http.PostAsync(loginEndpoint, null);

			if (!reply.IsSuccessStatusCode)
				return new JsonResult(new { ok = false, message = "Invalid email or password." });

			var root = JsonDocument.Parse(await reply.Content.ReadAsStringAsync()).RootElement;
			HttpContext.Session.SetString("Email", root.GetProperty("email").GetString() ?? typedEmail);
			HttpContext.Session.SetString("FullName", root.GetProperty("fullName").GetString() ?? typedEmail);
			HttpContext.Session.SetString("Role", selectedRole);
			HttpContext.Session.SetInt32("UserId", root.GetProperty("id").GetInt32());
			return new JsonResult(new { ok = true });
		}
	}
}