using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Web.Pages.Dashboard.Address
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly IAddressService _addressService;

        public EditModel(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [BindProperty(SupportsGet =true)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }


        public void OnGet()
        {
            var address = _addressService.GetAddress(Id);
            FirstName = address.FirstName;
            LastName = address.LastName;
            Email = address.Email;
            Phone = address.Phone;
            Country = address.Country;
            State = address.State;
            City = address.City;
            PostalCode = address.PostalCode;
            AddressLineOne = address.AddressLineOne;
            AddressLineTwo = address.AddressLineTwo;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var address = new ApplicationCore.Entities.Address()
            {
                Id = Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Phone = this.Phone,
                Country = this.Country,
                State = this.State,
                City = this.City,
                PostalCode = this.PostalCode,
                AddressLineOne = this.AddressLineOne,
                AddressLineTwo = this.AddressLineTwo,
                UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
            };

            _addressService.UpdateAddress(address);

            return RedirectToPage("/Dashboard/Index");
        }
    }
}
