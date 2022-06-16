using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Web.Pages.Dashboard.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;

        public IndexModel(IOrderService orderService, IOrderProductService orderProductService)
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
        }


        public IReadOnlyList<ApplicationCore.Entities.Order> YearOrders { get; set; }

        public List<SelectListItem> Years { get; set; }


        public void OnGet()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = _orderService.GetAllUserOrders(userId);

            Years = new();

            Years.Add(new SelectListItem() { Disabled = true, Selected = true, Value = "0", Text = "-- Select a year --" });

            foreach(var order in orders)
            {
                if(order.OrderDate != null)
                {
                    if (!Years.Exists(d => d.Text == order.OrderDate.Value.Year.ToString()))
                        Years.Add(new SelectListItem { Value = order.OrderDate.Value.Year.ToString(), Text = order.OrderDate.Value.Year.ToString() });
                }
            }
        }

        public PartialViewResult OnGetYearOrders(string year)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            YearOrders = _orderService.GetAllUserOrdersWithYear(year, userId);

            foreach(var order in YearOrders)
            {
                order.Order_Product = _orderProductService.GetOrderCurrentProducts(order.Id).ToList();
            }

            return Partial("_OrderListPartial", YearOrders);
        }
    }
}
