namespace Reserva.ViewModels
{
	using System.ComponentModel.DataAnnotations;

	namespace Reserva.ViewModels
	{
		public class ForgotPasswordViewModel
		{
			[Required(ErrorMessage = "Veuillez entrer une adresse email.")]
			[EmailAddress(ErrorMessage = "Adresse email invalide.")]
			public string Email { get; set; }
		}
	}

}
