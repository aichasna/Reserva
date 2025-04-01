using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using Reserva.ViewModels;
using System.Security.Claims;

namespace Reserva.Controllers
{
	public class PropertiesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _env;

		public PropertiesController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		[Authorize(Roles = "Propriétaire")]
		public IActionResult Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var proprietes = _context.Propriétés
				.Include(p => p.Photos)
				.Where(p => p.Utilisateur.Id == userId)
				.ToList();

			return View(proprietes);
		}

		[HttpGet]
		public IActionResult AjouterPropriete()
		{
			return View(new ProprieteViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> AjouterPropriete(ProprieteViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = _context.Utilisateurs.FirstOrDefault(u => u.Id == userId);

			if (user == null)
			{
				return Unauthorized();
			}

			var propriete = new Propriété
			{
				Nom = model.Nom,
				Description = model.Description,
				Type = model.Type,
				Adresse = model.Adresse,
				Ville = model.Ville,
				Pays = model.Pays,
				PrixParNuit = model.PrixParNuit,
				Capacite = model.Capacite,
				nbSalleDeBain = model.NbSalleDeBain,
				nbChambres = model.NbChambres,
				AnimauxAcceptes = model.AnimauxAcceptes,
				WifiDisponible = model.WifiDisponible,
				ParkingDisponible = model.ParkingDisponible,
				PiscineDisponible = model.PiscineDisponible,
				ClimatisationDisponible = model.ClimatisationDisponible,
				HeureArrivée = model.HeureArrivée,
				HeureDépart = model.HeureDépart,
				InstructionsArrivée = model.InstructionsArrivée,
				UtilisateurId = userId
			};

			_context.Propriétés.Add(propriete);
			await _context.SaveChangesAsync();

			if (model.Photos != null && model.Photos.Count > 0)
			{
				var fullName = $"{user.Prenom}_{user.Nom}".Replace(" ", "_");
				var directory = Path.Combine(_env.WebRootPath, "images", "propriétés");
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				foreach (var photo in model.Photos)
				{
					if (photo.Length > 0)
					{
						var fileExtension = Path.GetExtension(photo.FileName);
						var fileName = $"{Path.GetFileNameWithoutExtension(photo.FileName)}_{fullName}{fileExtension}";
						var filePath = Path.Combine(directory, fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await photo.CopyToAsync(stream);
						}

						var newPhoto = new Photo
						{
							CheminImage = $"/images/propriétés/{fileName}",
							ProprieteId = propriete.IdPropriete
						};

						_context.Photos.Add(newPhoto);
					}
				}

				await _context.SaveChangesAsync();
			}

			return RedirectToAction("Index", "Properties");
		}

		[HttpGet]
		public IActionResult SupprimerPropriete(int id)
		{
			var propriete = _context.Propriétés
				.Include(p => p.Photos)
				.FirstOrDefault(p => p.IdPropriete == id);

			if (propriete == null)
			{
				return NotFound();
			}

			// On renvoie la vue pour confirmation
			return View(propriete);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SupprimerProprieteConfirme(int id)
		{
			var propriete = _context.Propriétés
				.Include(p => p.Photos)
				.FirstOrDefault(p => p.IdPropriete == id);

			if (propriete == null)
			{
				return NotFound();
			}

			foreach (var photo in propriete.Photos)
			{
				var photoPath = Path.Combine(_env.WebRootPath, photo.CheminImage.TrimStart('/'));
				if (System.IO.File.Exists(photoPath))
				{
					System.IO.File.Delete(photoPath);
				}
			}

			_context.Propriétés.Remove(propriete);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Authorize(Roles = "Propriétaire")]
		public IActionResult EditProperties(int id)
		{
			// Rechercher la propriété dans la base de données
			var propriete = _context.Propriétés
				.Include(p => p.Photos)
				.FirstOrDefault(p => p.IdPropriete == id);

			if (propriete == null)
			{
				return NotFound();
			}

			// Mapper les données de la propriété vers le ViewModel
			var model = new ProprieteViewModel
			{
				IdPropriete = propriete.IdPropriete,
				Nom = propriete.Nom,
				Description = propriete.Description,
				Type = propriete.Type,
				Adresse = propriete.Adresse,
				Ville = propriete.Ville,
				Pays = propriete.Pays,
				PrixParNuit = propriete.PrixParNuit,
				Capacite = propriete.Capacite,
				NbSalleDeBain = propriete.nbSalleDeBain,
				NbChambres = propriete.nbChambres,
				AnimauxAcceptes = propriete.AnimauxAcceptes,
				WifiDisponible = propriete.WifiDisponible,
				ParkingDisponible = propriete.ParkingDisponible,
				PiscineDisponible = propriete.PiscineDisponible,
				ClimatisationDisponible = propriete.ClimatisationDisponible,
				HeureArrivée = propriete.HeureArrivée,
				HeureDépart = propriete.HeureDépart,
				InstructionsArrivée = propriete.InstructionsArrivée
			};

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = "Propriétaire")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProperties(ProprieteViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Rechercher la propriété dans la base de données
			var propriete = _context.Propriétés.FirstOrDefault(p => p.IdPropriete == model.IdPropriete);

			if (propriete == null)
			{
				return NotFound();
			}

			// Mettre à jour les champs
			propriete.Nom = model.Nom;
			propriete.Description = model.Description;
			propriete.Type = model.Type;
			propriete.Adresse = model.Adresse;
			propriete.Ville = model.Ville;
			propriete.Pays = model.Pays;
			propriete.PrixParNuit = model.PrixParNuit;
			propriete.Capacite = model.Capacite;
			propriete.nbSalleDeBain = model.NbSalleDeBain;
			propriete.nbChambres = model.NbChambres;
			propriete.AnimauxAcceptes = model.AnimauxAcceptes;
			propriete.WifiDisponible = model.WifiDisponible;
			propriete.ParkingDisponible = model.ParkingDisponible;
			propriete.PiscineDisponible = model.PiscineDisponible;
			propriete.ClimatisationDisponible = model.ClimatisationDisponible;
			propriete.HeureArrivée = model.HeureArrivée;
			propriete.HeureDépart = model.HeureDépart;
			propriete.InstructionsArrivée = model.InstructionsArrivée;

			// Gestion des nouvelles photos
			if (model.Photos != null && model.Photos.Count > 0)
			{
				var directory = Path.Combine(_env.WebRootPath, "images", "propriétés");
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				foreach (var photo in model.Photos)
				{
					if (photo.Length > 0)
					{
						var fileExtension = Path.GetExtension(photo.FileName);
						var fileName = $"{Path.GetFileNameWithoutExtension(photo.FileName)}_{propriete.IdPropriete}{fileExtension}";
						var filePath = Path.Combine(directory, fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await photo.CopyToAsync(stream);
						}

						var newPhoto = new Photo
						{
							CheminImage = $"/images/propriétés/{fileName}",
							ProprieteId = propriete.IdPropriete
						};

						_context.Photos.Add(newPhoto);
					}
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}



	}
}
