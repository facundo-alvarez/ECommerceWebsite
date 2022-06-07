using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Braintree;
using Infrastructure.Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;
using Web.Utility;

namespace Web.Pages.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IBraintreeGate _braintreeGate;
        private readonly IDiscountService _discountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderProductService _orderProductService;
        private readonly IOrderService _orderService;
        private readonly IAuthorizationService _authorizationService;

        public SummaryModel(IProductService productService, 
                            IBraintreeGate braintreeGate, 
                            IDiscountService discountService, 
                            IHttpContextAccessor httpContextAccessor, 
                            IOrderProductService orderProductService,
                            IOrderService orderService,
                            IAuthorizationService authorizationService)
        {
            _productService = productService;
            _braintreeGate = braintreeGate;
            _discountService = discountService;
            _httpContextAccessor = httpContextAccessor;
            _orderProductService = orderProductService;
            _orderService = orderService;
            _authorizationService = authorizationService;
        }

        public List<CartItem> CartItems { get; set; }

        public ApplicationCore.Entities.Order Order { get; set; }


        public IActionResult OnGet()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            if(Order == null)
            {
                return new NotFoundResult();
            }

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                var sessionItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                _orderProductService.SetOrderProducts(sessionItems, Order.Id);

                Order.SubTotal = _orderProductService.GetOrderSubtotal(Order.Id);
                Order.Total = Order.SubTotal;

                _orderService.UpdateOrder(Order);

                HttpContext.Session.Clear();
            }

            List<Item> Items = new ();
            CartItems = new();

            Order.Order_Product = _orderProductService.GetOrderCurrentProducts(Order.Id).ToList();

            foreach(var item in Order.Order_Product)
            {
                CartItems.Add(new CartItem()
                {
                    Product = _productService.GetProductById(item.ProductId),
                    Quantity = item.Quantity
                });
            }

            var gateway = _braintreeGate.GetGeteway();
            var clientToken = gateway.ClientToken.Generate();
            ViewData["ClientToken"] = clientToken;
            ViewData["Total"] = Order.Total;

            return Page();
        }

        public IActionResult OnPost(IFormCollection collection)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            string nonceFromTheClient = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = Order.Total,
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _braintreeGate.GetGeteway();
            
            Result<Transaction> result = gateway.Transaction.Sale(request);

            if(result.Target.ProcessorResponseText == "Approved")
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Order/Success");
            }
            else
            {

            }

            return RedirectToPage("/Order/Error");
        }

        public JsonResult OnGetDiscount(string prodCode)
        {
            if (_discountService.IsValid(prodCode))
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                Order = _orderService.GetUserCurrentOrder(userId);

                Order.DiscountCodeId = _discountService.GetDiscountByCode(prodCode).Id;
                Order.HasCupon = true;

                Order.Total = _discountService.GetDiscount(prodCode, Order.Total);

                _orderService.UpdateOrder(Order);

                return new JsonResult("True");
            }
            else
                return new JsonResult("False");
        }

        public void OnGetRemoveDiscount()
        {
            CartItems = new();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);
            _orderProductService.GetOrderCurrentProducts(Order.Id);

            Order.HasCupon = false;
            Order.DiscountCodeId = null;

            Order.SubTotal = _orderProductService.GetOrderSubtotal(Order.Id);
            Order.Total = Order.SubTotal;

            _orderService.UpdateOrder(Order);


            foreach (var item in Order.Order_Product)
            {
                CartItems.Add(new CartItem()
                {
                    Product = _productService.GetProductById(item.ProductId),
                    Quantity = item.Quantity
                });
            }
        }

        public PartialViewResult OnGetPartialTotal()
        {
            CartItems = new();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);
            _orderProductService.GetOrderCurrentProducts(Order.Id);

            foreach (var item in Order.Order_Product)
            {
                CartItems.Add(new CartItem()
                {
                    Product = _productService.GetProductById(item.ProductId),
                    Quantity = item.Quantity
                });
            }

            return Partial("_OrderTotalPartial", Order);
        }
    }
}
