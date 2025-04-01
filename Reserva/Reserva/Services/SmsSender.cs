using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Reserva.Services
{
	public class SmsSender : ISmsSender
	{
		private readonly string _accountSid;
		private readonly string _authToken;
		private readonly string _fromNumber;

		public SmsSender()
		{
			
			_accountSid = "";
			_authToken = "";
			_fromNumber = "";

			TwilioClient.Init(_accountSid, _authToken);
		}

		public async Task SendSmsAsync(string number, string message)
		{
			try
			{
				var messageResource = await MessageResource.CreateAsync(
					to: new PhoneNumber(number),
					from: new PhoneNumber(_fromNumber),
					body: message
				);

				Console.WriteLine($"SMS envoyé : {messageResource.Sid}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erreur lors de l'envoi du SMS : {ex.Message}");
				throw;
			}
		}
	}
}
