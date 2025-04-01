using Microsoft.AspNetCore.Identity;

namespace Reserva.Models
{
	public class Utilisateur : IdentityUser
	{
		public string Prenom { get; set; } // Obligatoire
		public string Nom { get; set; } // Obligatoire

		public DateTime? DateDeNaissance { get; set; } 
		public string? PhotoDeProfil { get; set; } 
		public string? Adresse { get; set; } 
		public string? Ville { get; set; } 
		public string? Pays { get; set; }
		public string NumeroDeTelephone { get; set; } // Obligatoire

		public DateTime DateDeCreation { get; set; } = DateTime.UtcNow;
		public bool EstVérifié { get; set; } = false;

		public ICollection<Role> Roles { get; set; } = new List<Role>();
		public ICollection<Réservation> Reservations { get; set; } = new List<Réservation>();
		public ICollection<Propriété> Proprietes { get; set; } = new List<Propriété>();

		public double? NoteMoyenneProprietaire => Proprietes
			.SelectMany(p => p.AvisPropriete)
			.DefaultIfEmpty()
			.Average(a => (double?)a?.Note);
	}
}
