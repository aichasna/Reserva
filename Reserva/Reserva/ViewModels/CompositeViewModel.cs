using Reserva.Models;

namespace Reserva.ViewModels
{
	public class CompositeViewModel
	{
		public LoginViewModel? Login { get; set; }
		public RegisterViewModel? Register { get; set; }
		public List<Propriété> TopProperties { get; set; }

	}
}
