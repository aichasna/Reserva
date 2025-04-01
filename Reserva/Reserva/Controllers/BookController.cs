using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using Reserva.Services;
using Twilio.TwiML.Messaging;


namespace Reserva.Controllers
{
	public class BookController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly EmailService _emailService;

		public BookController(ApplicationDbContext context, EmailService emailService)
		{
			_context = context;
			_emailService = emailService;
		}

		public IActionResult Index(int id)
		{
			var propriete = _context.Propriétés
				.Include(p => p.Photos)
				.Include(p => p.Equipements)
				.Include(p => p.AvisPropriete)
				.Include(p => p.Utilisateur)
				.Include(p => p.DatesDisponibilites)
				.FirstOrDefault(p => p.IdPropriete == id);

			if (propriete == null)
			{
				return NotFound();
			}

			return View(propriete);
		}

		[HttpGet]
		public IActionResult GetDisponibilitesFullCalendar(int id)
		{
			var today = DateTime.Today;
			var endDate = today.AddDays(365);

			// Récupère les plages réservées
			var reservedRanges = _context.Disponibilites
				.Where(d => d.ProprieteId == id)
				.Select(d => new
				{
					start = d.DateDebut.Date,
					end = d.DateFin.Date
				})
				.ToList();

			// Liste des jours réservés (ensemble pour vérification rapide)
			var reservedDays = new HashSet<DateTime>();
			foreach (var range in reservedRanges)
			{
				for (var day = range.start; day <= range.end; day = day.AddDays(1))
				{
					reservedDays.Add(day);
				}
			}

			var events = new List<object>();

			// Ajout des périodes réservées (rouge)
			foreach (var range in reservedRanges)
			{
				events.Add(new
				{
					start = range.start.ToString("yyyy-MM-dd"),
					end = range.end.AddDays(1).ToString("yyyy-MM-dd"),
					display = "background",
					backgroundColor = "#f44336", // Rouge pour réservé
					title = "Réservé"
				});
			}

			// Jours disponibles (vert) + Aujourd'hui (une autre couleur, ex: jaune)
			for (var day = today; day <= endDate; day = day.AddDays(1))
			{
				if (!reservedDays.Contains(day))
				{
					var color = (day == today) ? "#ffe000" : "#4caf50"; // Jaune pour aujourd'hui, vert pour disponible
					events.Add(new
					{
						start = day.ToString("yyyy-MM-dd"),
						end = day.AddDays(1).ToString("yyyy-MM-dd"),
						display = "background",
						backgroundColor = color,
						title = (day == today) ? "Aujourd'hui" : "Disponible"
					});
				}
			}

			return Json(events);
		}

		[HttpGet]
		public JsonResult GetReservedDates(int propertyId)
		{
			var reservedDates = _context.Disponibilites
				.Where(d => d.ProprieteId == propertyId)
				.Select(d => new
				{
					start = d.DateDebut.ToString("yyyy-MM-dd"),
					end = d.DateFin.ToString("yyyy-MM-dd")
				})
				.ToList();

			return Json(reservedDates);
		}

		[HttpPost]
		public async Task<IActionResult> Réserver(int proprieteId, DateTime dateDebut, DateTime dateFin)
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("Vous devez être connecté pour effectuer une réservation.");
			}

			var utilisateur = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (utilisateur == null)
			{
				return NotFound("Utilisateur introuvable.");
			}

			var propriete = _context.Propriétés.Include(p => p.DatesDisponibilites)
				.FirstOrDefault(p => p.IdPropriete == proprieteId);
			if (propriete == null)
			{
				return NotFound("Propriété introuvable.");
			}

			bool datesDisponibles = propriete.DatesDisponibilites.All(d =>
				(dateDebut < d.DateDebut || dateFin > d.DateFin) &&
				(dateDebut > d.DateFin || dateFin < d.DateDebut));
			if (!datesDisponibles)
			{
				return BadRequest("Les dates sélectionnées ne sont pas disponibles.");
			}

			int totalNuits = (int)(dateFin - dateDebut).TotalDays;
			double prixBase = totalNuits * propriete.PrixParNuit;
			const double tauxTaxes = 0.15; 
			const double fraisNettoyage = 50.0; 
			const double fraisService = 70.0;

			double montantTaxes = (prixBase + fraisNettoyage + fraisService) * tauxTaxes;
			double prixTotal = prixBase + fraisNettoyage + fraisService + montantTaxes;

			var reservation = new Réservation
			{
				DateDebut = dateDebut,
				DateFin = dateFin,
				Prix = prixTotal,
				Statut = "En attente",
				Utilisateur = utilisateur,
				Propriété = propriete
			};
			_context.Réservations.Add(reservation);

			var dateDisponibilite = new DateDisponibilité
			{
				DateDebut = dateDebut,
				DateFin = dateFin,
				ProprieteId = proprieteId
			};
			_context.Disponibilites.Add(dateDisponibilite);

			await _context.SaveChangesAsync();

			string sujet = "Confirmation de votre réservation";
			string contenu = $"<p>Bonjour {utilisateur.Prenom},</p><p>Votre réservation pour la propriété '{propriete.Nom}' du {dateDebut:dd/MM/yyyy} au {dateFin:dd/MM/yyyy} a été confirmée.</p><p>Montant total : {prixTotal:C}</p>";
			await _emailService.EnvoyerEmailAsync(utilisateur.Email, sujet, contenu);

			return RedirectToAction("Index", new { id = proprieteId });
		}
	}
}
