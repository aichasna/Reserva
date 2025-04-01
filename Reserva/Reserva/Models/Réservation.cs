using System.ComponentModel.DataAnnotations;

namespace Reserva.Models
{
	public class Réservation
	{
		[Key]
		public int IdReservation { get; set; }
		public DateTime DateDebut { get; set; }
		public DateTime DateFin { get; set; }
		public double Prix { get; set; }

		public string Statut { get; set; }

		public Utilisateur Utilisateur { get; set; }

		public Propriété Propriété { get; set; }
	}
}
