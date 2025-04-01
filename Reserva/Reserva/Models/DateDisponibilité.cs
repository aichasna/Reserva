using System.ComponentModel.DataAnnotations;

namespace Reserva.Models
{
	public class DateDisponibilité
	{
		[Key]
		public int IdDisponibilite { get; set; }
		public DateTime DateDebut { get; set; }
		public DateTime DateFin { get; set; }

		public int ProprieteId { get; set; }
		public Propriété Propriete { get; set; }
	}
}
