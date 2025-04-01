using Reserva.Models;

namespace Reserva.ViewModels
{
	public class RechercheProprieteViewModel
	{
		public string Destination { get; set; }
		public int? NombreAdultes { get; set; }
		public int? NombreEnfants { get; set; }
		public int? NombreBebes { get; set; }
		public List<string> TypeLogement { get; set; }
		public int? NombreChambres { get; set; }
		public int? NombreSallesDeBain { get; set; }
		public decimal? MinPrix { get; set; }
		public decimal? MaxPrix { get; set; }
		public List<string> Equipements { get; set; }
		public List<string> Installations { get; set; }
		public DateTime? DateDebut { get; set; }
		public DateTime? DateFin { get; set; }
		public List<Propriété> Resultats { get; set; }
		public string Tri { get; set; }

	}


}
