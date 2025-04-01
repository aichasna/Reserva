using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva.Models
{
	public class Propriété
	{
		[Key]
		public int IdPropriete { get; set; }

		[Required]
		[MaxLength(100)]
		public string Nom { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }

		[Required]
		public TypePropriete Type { get; set; }

		[Required]
		[MaxLength(200)]
		public string Adresse { get; set; }

		[Required]
		[MaxLength(100)]
		public string Ville { get; set; }

		[Required]
		[MaxLength(100)]
		public string Pays { get; set; }

		[Required]
		[Range(0, double.MaxValue, ErrorMessage = "Le prix par nuit doit être un nombre positif.")]
		public double PrixParNuit { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "La capacité doit être d'au moins 1 personne.")]
		public int Capacite { get; set; }
		[Required]
		public int nbSalleDeBain { get; set; }
		[Required]
		public int nbChambres { get; set; }

		public bool EstDisponible { get; set; } = true;
		public bool AnimauxAcceptes { get; set; }

		[Required]
		public string UtilisateurId { get; set; }

		[ForeignKey("UtilisateurId")]
		public Utilisateur Utilisateur { get; set; }

		public ICollection<Photo> Photos { get; set; } = new List<Photo>();

		public ICollection<Équipement> Equipements { get; set; } = new List<Équipement>();

		public ICollection<Réservation> Reservations { get; set; } = new List<Réservation>();

		public ICollection<DateDisponibilité> DatesDisponibilites { get; set; } = new List<DateDisponibilité>();

		public ICollection<Avis> AvisPropriete { get; set; } = new List<Avis>();

		public double? NoteMoyenne => AvisPropriete.Count > 0 ? (double?)AvisPropriete.Average(a => a.Note) : null;

        public bool WifiDisponible { get; set; } 

        public bool ParkingDisponible { get; set; } 

        public bool PiscineDisponible { get; set; } 

        [Range(0, 24)]
        public int HeureArrivée { get; set; } 

        [Range(0, 24)]
        public int HeureDépart { get; set; } 

        public bool ClimatisationDisponible { get; set; } 

        [MaxLength(1000)]
        public string InstructionsArrivée { get; set; } 
    }

	public enum TypePropriete
	{
		Appartement,
		Maison,
		Chalet,
		Studio,
		Villa,
		Hotel,
		Autre
	}
}
