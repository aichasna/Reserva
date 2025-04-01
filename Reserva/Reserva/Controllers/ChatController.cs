using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using Reserva.ViewModels;
using System.Security.Claims;

namespace Reserva.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

		[HttpGet]
		public async Task<IActionResult> GetMessages(string receiverId)
		{
			var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
				return BadRequest("Invalid IDs.");

			var messages = await _context.Messages
				.Include(m => m.Sender)
				.Include(m => m.Receiver)
				.Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
							(m.SenderId == receiverId && m.ReceiverId == senderId))
				.OrderBy(m => m.Timestamp)
				.Select(m => new
				{
					SenderId = m.SenderId,
					SenderName = m.Sender != null ? $"{m.Sender.Prenom} {m.Sender.Nom}" : "Inconnu",
					Content = string.IsNullOrEmpty(m.Content) ? "Message vide" : m.Content,
					Timestamp = m.Timestamp.ToString("g")
				})
				.ToListAsync();

			Console.WriteLine("[LOG] Messages récupérés :", messages);


			return Json(messages);
		}



		[HttpGet]
        public async Task<IActionResult> Conversations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid User ID.");

            var conversations = await _context.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationViewModel
                {
                    SenderId = g.Key,
                    SenderName = _context.Utilisateurs
                        .Where(u => u.Id == g.Key)
                        .Select(u => $"{u.Prenom} {u.Nom}")
                        .FirstOrDefault(),
                    LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault().Content,
                    LastTimestamp = g.Max(m => m.Timestamp)
                })
                .OrderByDescending(c => c.LastTimestamp)
                .ToListAsync();

            return View(conversations);
        }


    }
}
