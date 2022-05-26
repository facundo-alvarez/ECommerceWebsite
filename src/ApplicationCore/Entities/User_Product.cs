using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class User_Product
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
