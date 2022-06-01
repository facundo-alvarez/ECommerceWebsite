using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Braintree;
using Infrastructure.Braintree;
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

        public SummaryModel(IProductService productService, 
                            IBraintreeGate braintreeGate, 
                            IDiscountService discountService, 
                            IHttpContextAccessor httpContextAccessor, 
                            IOrderProductService orderProductService,
                            IOrderService orderService)
        {
            _productService = productService;
            _braintreeGate = braintreeGate;
            _discountService = discountService;
            _httpContextAccessor = httpContextAccessor;
            _orderProductService = orderProductService;
            _orderService = orderService;
        }


        public ApplicationCore.Entities.Order Order { get; set; }

        public List<Item> Items { get; set; }

        public List<CartItem> CartItems { get; set; }

        public List<Product> Products { get; set; }


        public void OnGet()
        {
            Items = new ();
            CartItems = new();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Order = _orderService.GetUserCurrentOrder(userId);

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }


            if (Order == null)
            {
                Order = new();

                Order.UserId = userId;

                _orderService.InsertOrder(Order);
                _orderService.SaveOrder(Order);

                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                _orderProductService.SetOrderProducts(Items, Order.Id);

                Order.SubTotal = _orderProductService.GetOrderSubtotal(Order.Id);

                //if(Order.HasCupon)
                //{
                //    Order.Total = _discountService.GetDiscount(Order.DiscountCode.Code, Order.SubTotal);
                //}
                //else
                //{
                //    Order.Total = Order.SubTotal;
                //}

                Order.Total = Order.SubTotal;

                Order.OrderStatus = "In process";

                _orderService.UpdateOrder(Order);
                _orderService.SaveOrder(Order);

            }
            else
            {
                Order.Order_Product = _orderProductService.GetOrderCurrentProducts(Order.Id);

                foreach(var product in Order.Order_Product)
                {
                    Items.Add(new Item()
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity
                    });
                }
            }

            foreach(var item in Items)
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
        }

        public IActionResult OnPost(IFormCollection collection)
        {
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

        public JsonResult OnGetDiscount(string code)
        {
            if (_discountService.IsValid(code))
            {
                return new JsonResult("Updated");
            }
            else
            {
                return new JsonResult("Invalid");
            }
        }
    }
}
