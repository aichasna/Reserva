using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using Reserva.ViewModels;
using System;
using System.Linq;

namespace Reserva.Controllers
{
	public class SearchController : Controller
	{
		private readonly ApplicationDbContext _context;

		public SearchController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			ViewData["ActiveItem"] = "Logements";
			var model = new RechercheProprieteViewModel
			{
				Resultats = _context.Propriétés
					.Include(p => p.Photos)
					.Include(p => p.AvisPropriete)
					.ToList()
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult Rechercher(RechercheProprieteViewModel model)
		{
			var query = _context.Propriétés
				.Include(p => p.Photos)
				.Include(p => p.AvisPropriete)
				.Include(p => p.DatesDisponibilites)
				.AsQueryable();

			// Calcul du nombre total d'invités
			int totalInvites = 0;
			if (model.NombreAdultes.HasValue)
				totalInvites += model.NombreAdultes.Value;
			if (model.NombreEnfants.HasValue)
				totalInvites += model.NombreEnfants.Value;
			if (model.NombreBebes.HasValue)
				totalInvites += model.NombreBebes.Value;

			// Filtrer par le nombre total d'invités
			if (totalInvites > 0)
			{
				query = query.Where(p => p.Capacite >= totalInvites);
			}

			// Filtrer par destination
			if (!string.IsNullOrEmpty(model.Destination))
			{
				query = query.Where(p => p.Ville.Contains(model.Destination) || p.Pays.Contains(model.Destination));
			}

			// Filtrer par type de logement
			if (model.TypeLogement != null && model.TypeLogement.Any())
			{
				var typesLogement = model.TypeLogement
					.Select(type => Enum.Parse<TypePropriete>(type))
					.ToList();

				query = query.Where(p => typesLogement.Contains(p.Type));
			}


			// Filtrer par nombre de chambres
			if (model.NombreChambres.HasValue)
			{
				query = query.Where(p => p.nbChambres >= model.NombreChambres.Value);
			}

			// Filtrer par nombre de salles de bain
			if (model.NombreSallesDeBain.HasValue)
			{
				query = query.Where(p => p.nbSalleDeBain >= model.NombreSallesDeBain.Value);
			}

			// Filtrer par prix
			if (model.MinPrix.HasValue)
			{
				query = query.Where(p => (decimal)p.PrixParNuit >= model.MinPrix.Value);
			}
			if (model.MaxPrix.HasValue)
			{
				query = query.Where(p => (decimal)p.PrixParNuit <= model.MaxPrix.Value);
			}

			// Filtrer par dates
			if (model.DateDebut.HasValue && model.DateFin.HasValue)
			{
				var dateDebut = model.DateDebut.Value.Date;
				var dateFin = model.DateFin.Value.Date;

				query = query.Where(p =>
					!p.DatesDisponibilites.Any(d =>
						(dateDebut >= d.DateDebut && dateDebut <= d.DateFin) || // Début de recherche chevauche une réservation
						(dateFin >= d.DateDebut && dateFin <= d.DateFin) ||    // Fin de recherche chevauche une réservation
						(dateDebut <= d.DateDebut && dateFin >= d.DateFin)     // Recherche inclut entièrement une réservation
					)
				);
			}

			// Équipements
			if (model.Equipements != null && model.Equipements.Any())
			{
				foreach (var equipement in model.Equipements)
				{
					query = query.Where(p => p.Equipements.Any(e => e.Nom == equipement));
				}
			}

			// Installations
			if (model.Installations != null && model.Installations.Any())
			{
				if (model.Installations.Contains("Parking"))
					query = query.Where(p => p.ParkingDisponible);
				if (model.Installations.Contains("Piscine"))
					query = query.Where(p => p.PiscineDisponible);
				if (model.Installations.Contains("Climatisation"))
					query = query.Where(p => p.ClimatisationDisponible);
			}

			model.Resultats = query
				.Include(p => p.Photos)
				.Include(p => p.AvisPropriete)
				.ToList();

			return View("Index", model);
		}

		public IActionResult Trier(string tri)
		{
			var query = _context.Propriétés
				.Include(p => p.Photos)
				.Include(p => p.AvisPropriete)
				.AsQueryable();

			// Appliquer le tri
			if (!string.IsNullOrEmpty(tri))
			{
				switch (tri)
				{
					case "price_asc":
						query = query.OrderBy(p => p.PrixParNuit);
						break;
					case "price_desc":
						query = query.OrderByDescending(p => p.PrixParNuit);
						break;
				}
			}

			var model = new RechercheProprieteViewModel
			{
				Resultats = query.ToList()
			};

			return View("Index", model);
		}

	}
}