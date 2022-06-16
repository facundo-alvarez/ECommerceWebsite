using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<User_Product> User_Product { get; set; }
        public ICollection<Address> Addresses { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
