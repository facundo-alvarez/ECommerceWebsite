using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Checkout
{
    public class PaymentModel : PageModel
    {
        private readonly IOrderService _orderService;

        public PaymentModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ApplicationCore.Entities.Order Order { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            if (Order == null)
            {
                return new NotFoundResult();
            }

            return Page();
        }
    }
}
