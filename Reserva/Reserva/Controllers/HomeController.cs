using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using Reserva.ViewModels;
using System.Diagnostics;

namespace Reserva.Controllers
{
    public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			var topProprietes = _context.Propriétés
				.Include(p => p.Photos)
				.Include(p => p.AvisPropriete)
				.Select(p => new
				{
					Propriete = p,
					NoteMoyenne = p.AvisPropriete.Any() ? p.AvisPropriete.Average(a => a.Note) : 0
				})
				.OrderByDescending(p => p.NoteMoyenne)
				.Take(6)
				.Select(p => p.Propriete)
				.ToList();

			var model = new CompositeViewModel
			{
				Login = new LoginViewModel(),
				Register = new RegisterViewModel(),
				TopProperties = topProprietes
			};

			return View(model);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
