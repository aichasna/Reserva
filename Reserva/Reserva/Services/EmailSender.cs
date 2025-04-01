using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reserva.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly string _smtpServer;
		private readonly int _smtpPort;
		private readonly string _smtpUser;
		private readonly string _smtpPassword;
		private readonly string _senderEmail;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(ILogger<EmailSender> logger)
		{
			_smtpServer = "smtp.gmail.com";
			_smtpPort = 587;
			_smtpUser = ""; // Adresse email
			_smtpPassword = "";
			_senderEmail = ""; // Adresse email de l'expéditeur
			_logger = logger;
		}

		public async Task SendEmailAsync(string email, string subject, string message)
		{
			_logger.LogInformation("Préparation de l'envoi de l'email à {Email} avec le sujet {Subject}", email, subject);

			var smtpClient = new SmtpClient(_smtpServer)
			{
				Port = _smtpPort,
				Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
				EnableSsl = true
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_senderEmail, "Reserva"),
				Subject = subject,
				Body = message,
				IsBodyHtml = true
			};

			mailMessage.To.Add(email);

			try
			{
				_logger.LogInformation("Envoi de l'email à {Email} via SMTP {SmtpServer}:{SmtpPort}", email, _smtpServer, _smtpPort);
				await smtpClient.SendMailAsync(mailMessage);
				_logger.LogInformation("Email envoyé avec succès à {Email}", email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erreur lors de l'envoi de l'email à {Email}", email);
				throw;
			}
		}
	}
}
