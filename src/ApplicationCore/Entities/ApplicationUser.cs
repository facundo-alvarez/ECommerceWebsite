using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<User_Product> User_Product { get; set; }

    }
}
