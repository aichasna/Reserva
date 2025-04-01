namespace Reserva.ViewModels
{
	public class ProfilViewModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime? BirthDate { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
		public string? ProfilePicture { get; set; }
		public DateTime CreationDate { get; set; }
		public bool IsVerified { get; set; }
	}
}
