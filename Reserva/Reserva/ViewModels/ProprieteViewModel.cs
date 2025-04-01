using Reserva.Models;

namespace Reserva.ViewModels
{
	public class ProprieteViewModel
	{
		public int? IdPropriete { get; set; }
		public string Nom { get; set; }
		public string Description { get; set; }
		public TypePropriete Type { get; set; }
		public string Adresse { get; set; }
		public string Ville { get; set; }
		public string Pays { get; set; }
		public double PrixParNuit { get; set; }
		public int Capacite { get; set; }
		public int NbSalleDeBain { get; set; }
		public int NbChambres { get; set; }
		public bool AnimauxAcceptes { get; set; }
		public bool WifiDisponible { get; set; }
		public bool ParkingDisponible { get; set; }
		public bool PiscineDisponible { get; set; }
		public bool ClimatisationDisponible { get; set; }
		public int HeureArrivée { get; set; }
		public int HeureDépart { get; set; }
		public string InstructionsArrivée { get; set; }

		public List<IFormFile> Photos { get; set; }
	}

}
