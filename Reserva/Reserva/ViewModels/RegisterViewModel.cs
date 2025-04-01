namespace Reserva.ViewModels
{
	using System.ComponentModel.DataAnnotations;

	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

		[Required]
		public string Prenom { get; set; }

		[Required]
		public string Nom { get; set; }

		[Required]
		[Phone]
		public string NumeroDeTelephone { get; set; }

        public bool IsOwner { get; set; }
    }
}
