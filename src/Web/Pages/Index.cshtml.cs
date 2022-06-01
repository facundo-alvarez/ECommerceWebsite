using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.Utility;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;

        public IndexModel(IHttpContextAccessor httpContextAccessor, IOrderService orderService, IOrderProductService orderProductService)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _orderProductService = orderProductService;
        }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);

                if (order != null)
                {
                    if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                    {
                        var sessionItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                        _orderProductService.SetOrderProducts(sessionItems, order.Id);

                        order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                        order.Total = order.SubTotal;

                        _orderService.UpdateOrder(order);
                        
                        HttpContext.Session.Clear();
                    }
                }
                else
                {
                    if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                    {
                        order = new();
                        order.UserId = userId;

                        _orderService.InsertOrder(order);
                        _orderService.SaveOrder(order);

                        var sessionItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                        _orderProductService.SetOrderProducts(sessionItems, order.Id);
                        HttpContext.Session.Clear();

                        order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                        order.Total = order.SubTotal;
                        order.OrderStatus = "In process";

                        _orderService.UpdateOrder(order);
                        _orderService.SaveOrder(order);
                    }
                }
            }
        }
    }
}