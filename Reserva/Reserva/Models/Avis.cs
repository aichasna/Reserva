using System.ComponentModel.DataAnnotations;

namespace Reserva.Models
{
	public class Avis
	{
		[Key]
		public int IdAvis { get; set; }

		[Required]
		public int ProprieteId { get; set; }

		public Propriété Propriete { get; set; }

		[Required]
		public string UtilisateurId { get; set; }

		public Utilisateur Utilisateur { get; set; }

		[Required]
		[Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5.")]
		public int Note { get; set; }

		[MaxLength(500)]
		public string Commentaire { get; set; }

		public DateTime DateAvis { get; set; } = DateTime.Now;
	}
}
