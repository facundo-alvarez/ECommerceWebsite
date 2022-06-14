using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Web.Utility;

namespace Web.Pages.Checkout
{
    public class AddressModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;


        public AddressModel(IOrderService orderService, IAddressService addressService)
        {
            _orderService = orderService;
            _addressService = addressService;
        }


        public IReadOnlyList<ApplicationCore.Entities.Address> Addresses { get; set; }

        [Required, BindProperty]
        public string FirstName { get; set; }
        [Required, BindProperty]
        public string LastName { get; set; }
        [Required, BindProperty]
        public string Email { get; set; }
        [Required, BindProperty]
        public string Phone { get; set; }
        [Required, BindProperty]
        public string Country { get; set; }
        [Required, BindProperty]
        public string State { get; set; }
        [Required, BindProperty]
        public string City { get; set; }
        [Required, BindProperty]
        public string PostalCode { get; set; }
        [Required, BindProperty]
        public string AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }

        public ApplicationCore.Entities.Order Order { get; set; }

        public Address Address { get; set; }



        public IActionResult OnGet()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            if (Order == null)
            {
                return new NotFoundResult();
            }

            Addresses = _addressService.GetAddressesWithUserId(userId);

            if(Order.AddressId != null)
            {
                Address = _addressService.GetAddress((int)Order.AddressId);
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var address = new ApplicationCore.Entities.Address()
            {
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

            _addressService.AddAddress(address);

            return RedirectToAction("Index");
        }

        public PartialViewResult OnGetApplyAddress(int addressId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);
            Address = _addressService.GetAddress(addressId);

            Order.AddressId = addressId;
            _orderService.UpdateOrder(Order);

            return Partial("_AddressCheckPartial", Address);
        }

        public PartialViewResult OnGetChangeAddress()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            Order.AddressId = null;
            _orderService.UpdateOrder(Order);

            Addresses = _addressService.GetAddressesWithUserId(userId);

            return Partial("_AddressListPartial", Addresses);
        }
    }
}
