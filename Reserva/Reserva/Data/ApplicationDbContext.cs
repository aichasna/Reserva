using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reserva.Models;
namespace Reserva.Data
{
    
	public class ApplicationDbContext : IdentityDbContext<Utilisateur>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<Propriété> Propriétés { get; set; }
		public DbSet<Réservation> Réservations { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Équipement> Equipements { get; set; }
		public DbSet<DateDisponibilité> Disponibilites { get; set; }
		public DbSet<Avis> AvisProprietes { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
		public DbSet<Message> Messages { get; set; }




		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Avis>()
                .HasOne(a => a.Propriete)
                .WithMany(p => p.AvisPropriete)
                .HasForeignKey(a => a.ProprieteId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Propriété>()
                .HasOne(p => p.Utilisateur)
                .WithMany(u => u.Proprietes)
                .HasForeignKey(p => p.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Photo>()
				.HasOne(p => p.Propriete)
				.WithMany(pr => pr.Photos)
				.HasForeignKey(p => p.ProprieteId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany()
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany()
				.HasForeignKey(m => m.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);

		}

	}
}
