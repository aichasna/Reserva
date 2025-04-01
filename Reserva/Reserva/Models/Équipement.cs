using System.ComponentModel.DataAnnotations;

namespace Reserva.Models
{
	public class Équipement
	{
		[Key]
		public int IdEquipement { get; set; }
		public string Nom { get; set; }

		[Required]
		public int ProprieteId { get; set; }
		public Propriété Propriete { get; set; }
	}
}
