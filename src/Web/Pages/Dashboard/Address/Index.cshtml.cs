using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Dashboard.Address
{
    public class IndexModel : PageModel
    {
        private readonly IAddressService _addressService;

        public IndexModel(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public IReadOnlyList<ApplicationCore.Entities.Address> Addresses { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Addresses = _addressService.GetAddressesWithUserId(userId);
        }

        public PartialViewResult OnGetDelete(int addressId)
        {
            _addressService.RemoveAddress(addressId);

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Addresses = _addressService.GetAddressesWithUserId(userId);

            return Partial("_AddressesPartial", Addresses);
        }
    }
}
