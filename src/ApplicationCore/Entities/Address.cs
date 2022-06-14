namespace ApplicationCore.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }


        public ICollection<Order> Orders { get; set; }

        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }

        public string FullName() => FirstName + " " + LastName;
    }
}
