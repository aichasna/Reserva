using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Reserva.Data;
using Reserva.Models;
using System.Security.Claims;

namespace Reserva.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

		public async Task SendMessage(string receiverId, string content)
		{
			var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			// Ajout des logs pour vérifier les valeurs
			Console.WriteLine($"[LOG] Tentative d'envoi d'un message");
			Console.WriteLine($"[LOG] SenderId: {senderId}");
			Console.WriteLine($"[LOG] ReceiverId: {receiverId}");
			Console.WriteLine($"[LOG] Content: {content}");

			if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId) || string.IsNullOrWhiteSpace(content))
			{
				Console.WriteLine("[LOG] Paramètres manquants ou invalides");
				throw new ArgumentException("SenderId, ReceiverId ou Content est manquant.");
			}

			var sender = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Id == senderId);
			var receiver = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Id == receiverId);

			if (sender == null || receiver == null)
			{
				Console.WriteLine("[LOG] Sender ou Receiver introuvable dans la base de données");
				return;
			}

			var message = new Message
			{
				SenderId = senderId,
				ReceiverId = receiverId,
				Content = content,
				Timestamp = DateTime.UtcNow,
				Sender = sender,
				Receiver = receiver
			};

			_context.Messages.Add(message);
			await _context.SaveChangesAsync();

			// Log après sauvegarde
			Console.WriteLine("[LOG] Message sauvegardé dans la base de données");

			var groupName = GetGroupName(senderId, receiverId);

			// Log avant l'envoi au groupe
			Console.WriteLine($"[LOG] Envoi au groupe {groupName}");

			await Clients.Group(groupName).SendAsync("ReceiveMessage", new
			{
				SenderId = senderId,
				SenderName = $"{sender.Prenom} {sender.Nom}",
				Content = content,
				Timestamp = message.Timestamp.ToString("g")
			});

			// Log après envoi réussi
			Console.WriteLine("[LOG] Message envoyé au groupe");
			Console.WriteLine($"[LOG] Objet envoyé au client : SenderId={senderId}, SenderName={sender.Prenom} {sender.Nom}, Content={content}, Timestamp={message.Timestamp.ToString("g")}");

		}



		public async Task JoinGroup(string receiverId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
                return;

            var groupName = GetGroupName(senderId, receiverId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string receiverId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
                return;

            var groupName = GetGroupName(senderId, receiverId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }


        private string GetGroupName(string user1, string user2)
        {
            return string.Compare(user1, user2) < 0 ? $"{user1}-{user2}" : $"{user2}-{user1}";
        }
    }
}
