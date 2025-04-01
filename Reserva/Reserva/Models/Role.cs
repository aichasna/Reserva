using Microsoft.AspNetCore.Identity;

namespace Reserva.Models
{
	public class Role : IdentityRole
	{
		public string Description { get; set; }

		public ICollection<Utilisateur> Utilisateurs { get; set; }
	}

}
