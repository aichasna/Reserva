using System.ComponentModel.DataAnnotations;

namespace Reserva.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Le mot de passe doit comporter au moins {2} caractères.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
		public string ConfirmPassword { get; set; }
	}

}
