using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva.Models
{
	public class Photo
	{
		[Key]
		public int IdPhoto { get; set; }

		[Required]
		[MaxLength(250)]
		public string CheminImage { get; set; }

		[Required]
		public int ProprieteId { get; set; }

		[ForeignKey("ProprieteId")]
		public Propriété Propriete { get; set; }
	}
}
