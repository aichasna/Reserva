using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using System.Security.Claims;

namespace Reserva.Controllers
{
	[Authorize]
	public class BookingsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public BookingsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var reservations = _context.Réservations
				.Include(r => r.Propriété) 
				.Include(r => r.Propriété.Photos) 
				.Where(r => r.Utilisateur.Id == userId)
				.ToList();

			return View(reservations);
		}

		[HttpPost]
		public IActionResult AnnulerReservation(int id)
		{
			var reservation = _context.Réservations
				.Include(r => r.Propriété)
				.FirstOrDefault(r => r.IdReservation == id);

			if (reservation == null)
			{
				return NotFound();
			}

			// Mettre à jour le statut de la réservation
			reservation.Statut = "Annulé";

			// Supprimer les dates de la réservation dans DatesDisponibilites
			var datesDisponibles = _context.Disponibilites
				.Where(d => d.ProprieteId == reservation.Propriété.IdPropriete &&
							d.DateDebut == reservation.DateDebut &&
							d.DateFin == reservation.DateFin)
				.ToList();

			_context.Disponibilites.RemoveRange(datesDisponibles);

			// Sauvegarder les modifications
			_context.SaveChanges();

			TempData["SuccessMessage"] = "La réservation a été annulée avec succès.";

			return RedirectToAction("Index");
		}

	}
}
