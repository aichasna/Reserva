using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Reserva.Services
{
	public class EmailService
	{
		private readonly string _apiKey;
		private readonly string _fromEmail;
		private readonly string _fromName;

		public EmailService(IConfiguration configuration)
		{
			var sendGridConfig = configuration.GetSection("SendGrid");
			_apiKey = sendGridConfig["ApiKey"];
			_fromEmail = sendGridConfig["FromEmail"];
			_fromName = sendGridConfig["FromName"];
		}

		public async Task EnvoyerEmailAsync(string destinataire, string sujet, string contenuHtml)
		{
			var client = new SendGridClient(_apiKey);
			var from = new EmailAddress(_fromEmail, _fromName);
			var to = new EmailAddress(destinataire);
			var message = MailHelper.CreateSingleEmail(from, to, sujet, contenuHtml, contenuHtml);

			Console.WriteLine("Tentative d'envoi d'un e-mail...");
			Console.WriteLine($"Destinataire : {destinataire}");
			Console.WriteLine($"Sujet : {sujet}");

			var response = await client.SendEmailAsync(message);

			Console.WriteLine($"Statut de la réponse : {response.StatusCode}");
			if (!response.IsSuccessStatusCode)
			{
				var body = await response.Body.ReadAsStringAsync();
				Console.WriteLine($"Erreur dans la réponse : {body}");
			}
			else
			{
				Console.WriteLine("E-mail envoyé avec succès.");
			}
		}

	}
}
