namespace Reserva.ViewModels
{
	public class VerifyTwoFactorViewModel
	{
		public string Code { get; set; }
		public bool RememberMe { get; set; }
		public string? ReturnUrl { get; set; }
	}

}
