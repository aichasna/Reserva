using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Reserva.Models;
using Reserva.Services;
using Reserva.ViewModels;
using System.Security.Claims;

namespace Reserva.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager,
            ISmsSender smsSender,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _smsSender = smsSender;
            _emailSender = emailSender;
        }

        #region Index

        [HttpGet]
        public IActionResult Index()
        {
            var model = new CompositeViewModel
            {
                Login = new LoginViewModel(),
                Register = new RegisterViewModel()
            };
            return View(model);
        }

        #endregion


        [HttpGet]
        public IActionResult LoginRegister()
        {
            ViewData["HideHeaderFooter"] = true;
            var model = new CompositeViewModel
            {
                Login = new LoginViewModel(),
                Register = new RegisterViewModel()
            };
            return View(model);
        }


        #region Register


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CompositeViewModel model)
        {
            if (!ModelState.IsValid || model.Register == null)
            {
                _logger.LogError("Modèle invalide : {errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View("LoginRegister", model);
            }

            if (!model.Register.Email.Contains("@") || !model.Register.Email.Contains("."))
            {
                ModelState.AddModelError("Register.Email", "L'adresse e-mail est invalide.");
            }

            if (model.Register.NumeroDeTelephone.Length != 10 || !model.Register.NumeroDeTelephone.All(char.IsDigit))
            {
                ModelState.AddModelError("Register.NumeroDeTelephone", "Le numéro de téléphone doit comporter 10 chiffres.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Modèle invalide après validations personnalisées.");
                return View("LoginRegister", model);
            }

            var user = new Utilisateur
            {
                UserName = model.Register.Email,
                Email = model.Register.Email,
                Prenom = model.Register.Prenom,
                Nom = model.Register.Nom,
                NumeroDeTelephone = model.Register.NumeroDeTelephone,
                PhoneNumber = model.Register.NumeroDeTelephone,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                EmailConfirmed = false,
                DateDeCreation = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Register.Password);
            if (result.Succeeded)
            {

                if (model.Register.IsOwner)
                {
                    await _userManager.AddToRoleAsync(user, "Propriétaire");
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirmation d'inscription",
                    $"Cliquez sur ce lien pour confirmer votre inscription : <a href='{confirmationLink}'>Confirmer</a>");

                TempData["SuccessMessage"] = "Inscription réussie ! Veuillez vérifier vos courriels pour confirmer votre compte.";
                return View("LoginRegister", model); // Retourner la même vue pour afficher le message
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("LoginRegister", model);
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Utilisateur introuvable.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "E-mail confirmé avec succès. Vous pouvez maintenant vous connecter.";
                return RedirectToAction(nameof(LoginRegister));
            }

            TempData["Error"] = "Erreur lors de la confirmation de l'e-mail.";
            return RedirectToAction("Index", "Home");
        }


        #endregion

        #region Login



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Login")] CompositeViewModel model)
        {
            if (!ModelState.IsValid || model.Login == null)
            {
                _logger.LogError("Modèle invalide : {errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View("LoginRegister", new CompositeViewModel { Login = new LoginViewModel(), Register = new RegisterViewModel() });
            }

            _logger.LogInformation("Recherche de l'utilisateur avec l'email : {Email}", model.Login.Email);
            var user = await _userManager.FindByEmailAsync(model.Login.Email);

            if (user == null)
            {
                _logger.LogWarning("Aucun utilisateur trouvé avec l'email : {Email}", model.Login.Email);
                ModelState.AddModelError("Login.Email", "Aucun compte trouvé avec cette adresse e-mail.");
                return View("LoginRegister", model);
            }

            _logger.LogInformation("Vérification du mot de passe pour l'utilisateur : {Email}", model.Login.Email);
            if (!await _userManager.CheckPasswordAsync(user, model.Login.Password))
            {
                _logger.LogWarning("Mot de passe incorrect pour l'utilisateur : {Email}", model.Login.Email);
                ModelState.AddModelError("Login.Password", "Le mot de passe est incorrect.");
                return View("LoginRegister", model);
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email non confirmé pour l'utilisateur : {Email}", model.Login.Email);
                ModelState.AddModelError("Login.Email", "Veuillez confirmer votre adresse e-mail avant de vous connecter.");
                return View("LoginRegister", model);
            }

            _logger.LogInformation("État de l'utilisateur : EmailConfirmed={EmailConfirmed}, TwoFactorEnabled={TwoFactorEnabled}, LockoutEnd={LockoutEnd}",
                user.EmailConfirmed, user.TwoFactorEnabled, user.LockoutEnd);

            _logger.LogInformation("Tentative de connexion : Username={Username}, RememberMe={RememberMe}, LockoutOnFailure={LockoutOnFailure}",
                user.UserName, model.Login.RememberMe, false);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Login.Password, model.Login.RememberMe, lockoutOnFailure: false);

            _logger.LogError("État de connexion : Succeeded={Succeeded}, RequiresTwoFactor={RequiresTwoFactor}, IsLockedOut={IsLockedOut}",
                result.Succeeded, result.RequiresTwoFactor, result.IsLockedOut);

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Le compte est verrouillé pour l'utilisateur : {Email}", model.Login.Email);
                ModelState.AddModelError("", "Votre compte est verrouillé. Réessayez plus tard.");
                return View("LoginRegister", model);
            }

            if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("L'utilisateur {Email} a activé la vérification en deux étapes.", model.Login.Email);
                return RedirectToAction(nameof(SendTwoFactorCode), new { rememberMe = model.Login.RememberMe });
            }

            if (result.Succeeded)
            {
                _logger.LogInformation("Connexion réussie pour l'utilisateur : {Email}", model.Login.Email);
                return RedirectToAction("Index", "Home");
            }

            _logger.LogError("Erreur inattendue lors de la connexion pour l'utilisateur : {Email}", model.Login.Email);
            ModelState.AddModelError("", "Erreur inattendue lors de la connexion.");
            return View("LoginRegister", model);
        }












        #endregion

        #region Two-Factor Authentication

        private string ConvertToInternationalFormat(string phoneNumber, string countryCode = "+1")
        {
            if (!phoneNumber.StartsWith("+"))
            {
                return countryCode + phoneNumber.Trim();
            }
            return phoneNumber;
        }

        [HttpGet]
        public async Task<IActionResult> SendTwoFactorCode(bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogError("Utilisateur 2FA introuvable.");
                return RedirectToAction(nameof(LoginRegister));
            }

            // Vérifiez que le numéro de téléphone existe
            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                _logger.LogError("Numéro de téléphone introuvable pour l'utilisateur : {UserId}", user.Id);
                TempData["Error"] = "Aucun numéro de téléphone associé à votre compte pour 2FA.";
                return RedirectToAction(nameof(LoginRegister));
            }

            // Génération et envoi du code
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogError("Erreur lors de la génération du code 2FA.");
                return RedirectToAction(nameof(LoginRegister));
            }

            await _smsSender.SendSmsAsync(user.PhoneNumber, $"Votre code de vérification est : {code}");
            _logger.LogInformation("Code 2FA envoyé au numéro {PhoneNumber}", user.PhoneNumber);

            return View("VerifyTwoFactor", new VerifyTwoFactorViewModel { RememberMe = rememberMe });
        }




        [HttpGet]
        public IActionResult VerifyTwoFactor(bool rememberMe, string? returnUrl = null)
        {
            var model = new VerifyTwoFactorViewModel
            {
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogError("Utilisateur 2FA introuvable.");
                return RedirectToAction(nameof(LoginRegister));
            }

            var result = await _signInManager.TwoFactorSignInAsync("Phone", model.Code, model.RememberMe, rememberClient: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("2FA réussie pour l'utilisateur : {UserId}", user.Id);
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Compte verrouillé après plusieurs tentatives 2FA.");
                TempData["Error"] = "Votre compte est verrouillé. Veuillez réessayer plus tard.";
                return RedirectToAction(nameof(LoginRegister));
            }

            ModelState.AddModelError("", "Code de vérification invalide.");
            return View(model);
        }





        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

       


        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["Error"] = "Erreur lors de la connexion avec Google.";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

            if (user == null)
            {
                user = new Utilisateur
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Prenom = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    Nom = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    NumeroDeTelephone = "0000000000",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(returnUrl ?? "/");
        }

        #endregion


        #region Password

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewData["HideHeaderFooter"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Veuillez entrer une adresse e-mail valide.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(email, "Réinitialisation de mot de passe",
                    $"Veuillez réinitialiser votre mot de passe en cliquant sur ce lien : <a href='{callbackUrl}'>Réinitialiser le mot de passe</a>");
            }

            TempData["SuccessMessage"] = "Si un compte existe pour cet e-mail, un lien de réinitialisation a été envoyé. Veuillez vérifier vos courriels.";
            return View();
        }





        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Message"] = "Votre mot de passe a été réinitialisé avec succès. Vous serez redirigé vers la page d'accueil.";
                return RedirectToAction("ResetPasswordSuccess");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Votre mot de passe a été réinitialisé avec succès. Vous serez redirigé vers la page d'accueil.";
                return RedirectToAction("ResetPasswordSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordSuccess()
        {
            ViewData["HideHeaderFooter"] = true;
            return View();
        }

        #endregion


        #region Profil

        [HttpGet]
        public async Task<IActionResult> Profil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LoginRegister"); // Rediriger vers la page de login si non connecté
            }

            var model = new ProfilViewModel
            {
                FirstName = user.Prenom,
                LastName = user.Nom,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.DateDeNaissance,
                Address = user.Adresse,
                City = user.Ville,
                Country = user.Pays,
                ProfilePicture = user.PhotoDeProfil ?? "/images/default-user.png",
                CreationDate = user.DateDeCreation,
                IsVerified = user.EmailConfirmed
            };

            ViewData["HideHeaderFooter"] = true;
            return View("Profil", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfil(ProfilViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LoginRegister");
            }

            // On ne modifie pas Email, FirstName (Prenom) et LastName (Nom) car ils sont en lecture seule
            // user.Email = user.Email;
            // user.Prenom = user.Prenom;
            // user.Nom = user.Nom;

            user.PhoneNumber = model.PhoneNumber;
            user.DateDeNaissance = model.BirthDate;
            user.Adresse = model.Address;
            user.Ville = model.City;
            user.Pays = model.Country;

            // Gestion de l'image si un fichier a été uploadé
            var files = Request.Form.Files;
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                if (file.Length > 0)
                {
                    // Chemin où stocker l'image, par exemple: wwwroot/images/profiles
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Stocker le chemin relatif dans la base de données
                    user.PhotoDeProfil = "/images/profiles/" + fileName;
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Profil", model);
            }

            // Une fois mis à jour, on recharge les infos pour les afficher à jour
            TempData["SuccessMessage"] = "Profil mis à jour avec succès!";
            return RedirectToAction("Profil");
        }

        #endregion



        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
