using Microsoft.AspNetCore.Identity;
using Reserva.Models;

namespace Reserva.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Utilisateur>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            // Ajouter des rôles par défaut
            string[] roleNames = { "Administrateur", "Utilisateur", "Propriétaire"};

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }

            // Ajouter un utilisateur administrateur par défaut
            var adminEmail = "admin@exemple.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Utilisateur
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Prenom = "Admin",
                    Nom = "Utilisateur",
                    DateDeNaissance = new DateTime(1985, 4, 15),
                    PhotoDeProfil = "admin.jpg",
                    Adresse = "123 Admin Street",
                    Ville = "Paris",
                    Pays = "France",
                    NumeroDeTelephone = "0123456789",
                    EstVérifié = true
                };

                var result = await userManager.CreateAsync(adminUser, "MotDePasseFort123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrateur");
                }
            }

            // Ajouter d'autres utilisateurs
            var utilisateur1 = new Utilisateur
            {
                UserName = "utilisateur1@exemple.com",
                Email = "utilisateur1@exemple.com",
                EmailConfirmed = true,
                Prenom = "Alice",
                Nom = "Dupont",
                DateDeNaissance = new DateTime(1990, 5, 15),
                PhotoDeProfil = "alice.jpg",
                Adresse = "123 Rue Principale",
                Ville = "Paris",
                Pays = "France",
                NumeroDeTelephone = "0102030405",
                EstVérifié = true
            };

            var utilisateur2 = new Utilisateur
            {
                UserName = "utilisateur2@exemple.com",
                Email = "utilisateur2@exemple.com",
                EmailConfirmed = true,
                Prenom = "Bob",
                Nom = "Martin",
                DateDeNaissance = new DateTime(1985, 8, 23),
                PhotoDeProfil = "bob.jpg",
                Adresse = "456 Avenue des Champs",
                Ville = "Lyon",
                Pays = "France",
                NumeroDeTelephone = "0607080910",
                EstVérifié = false
            };

            var utilisateur3 = new Utilisateur
            {
                UserName = "utilisateur3@exemple.com",
                Email = "utilisateur3@exemple.com",
                EmailConfirmed = true,
                Prenom = "Caroline",
                Nom = "Lemoine",
                DateDeNaissance = new DateTime(1992, 12, 3),
                PhotoDeProfil = "caroline.jpg",
                Adresse = "789 Boulevard Saint-Germain",
                Ville = "Marseille",
                Pays = "France",
                NumeroDeTelephone = "0708091011",
                EstVérifié = true
            };

            // Ajouter les utilisateurs dans la base de données
            await userManager.CreateAsync(utilisateur1, "MotDePasseFort123!");
            await userManager.CreateAsync(utilisateur2, "MotDePasseFort123!");
            await userManager.CreateAsync(utilisateur3, "MotDePasseFort123!");

            // Ajouter un rôle Utilisateur
            var userRole = await roleManager.FindByNameAsync("Utilisateur");
            if (userRole != null)
            {
                await userManager.AddToRoleAsync(utilisateur1, "Utilisateur");
                await userManager.AddToRoleAsync(utilisateur2, "Utilisateur");
                await userManager.AddToRoleAsync(utilisateur3, "Utilisateur");
            }

            // Ajouter des propriétés
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!dbContext.Propriétés.Any())
            {
                dbContext.Propriétés.AddRange(
                    new Propriété
                    {
                        Nom = "Villa en Bord de Mer",
                        Description = "Une magnifique villa avec une vue imprenable sur la mer.",
                        Type = TypePropriete.Villa,
                        Adresse = "123 Rue de la Plage",
                        Ville = "Nice",
                        Pays = "France",
                        PrixParNuit = 350.0,
                        Capacite = 6,
                        nbSalleDeBain = 3,
                        nbChambres = 4,
                        EstDisponible = true,
                        AnimauxAcceptes = false,
                        UtilisateurId = utilisateur1.Id, // ID de l'utilisateur
                        WifiDisponible = true,
                        ParkingDisponible = true,
                        PiscineDisponible = true,
                        HeureArrivée = 15,
                        HeureDépart = 11,
                        ClimatisationDisponible = true,
                        InstructionsArrivée = "Code de la porte : 1234"
                    },
                    new Propriété
                    {
                        Nom = "Chalet en Montagne",
                        Description = "Un chalet confortable parfait pour les vacances d'hiver.",
                        Type = TypePropriete.Chalet,
                        Adresse = "456 Chemin des Neiges",
                        Ville = "Chamonix",
                        Pays = "France",
                        PrixParNuit = 250.0,
                        Capacite = 8,
                        nbSalleDeBain = 2,
                        nbChambres = 5,
                        EstDisponible = true,
                        AnimauxAcceptes = true,
                        UtilisateurId = utilisateur2.Id, // ID de l'utilisateur
                        WifiDisponible = true,
                        ParkingDisponible = false,
                        PiscineDisponible = false,
                        HeureArrivée = 16,
                        HeureDépart = 10,
                        ClimatisationDisponible = false,
                        InstructionsArrivée = "Récupérer les clés chez le voisin au 450 Chemin des Neiges."
                    },
                    new Propriété
                    {
                        Nom = "Appartement au Centre-ville",
                        Description = "Un appartement moderne proche de toutes les commodités.",
                        Type = TypePropriete.Appartement,
                        Adresse = "789 Avenue des Champs",
                        Ville = "Paris",
                        Pays = "France",
                        PrixParNuit = 120.0,
                        Capacite = 2,
                        nbSalleDeBain = 1,
                        nbChambres = 1,
                        EstDisponible = true,
                        AnimauxAcceptes = false,
                        UtilisateurId = utilisateur3.Id, // ID de l'utilisateur
                        WifiDisponible = true,
                        ParkingDisponible = false,
                        PiscineDisponible = false,
                        HeureArrivée = 14,
                        HeureDépart = 12,
                        ClimatisationDisponible = true,
                        InstructionsArrivée = "Utilisez l'interphone pour entrer, puis prenez l'ascenseur au 4ème étage."
                    }
                );

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
