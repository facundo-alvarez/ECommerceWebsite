using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Braintree;
using Infrastructure.Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Checkout
{
    public class ConfirmModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;
        private readonly IProductService _productService;
        private readonly IAddressService _addressService;
        private readonly IDiscountService _discountService;
        private readonly IBraintreeGate _braintreeGate;

        public ConfirmModel(IOrderService orderService, 
                            IOrderProductService orderProductService, 
                            IProductService productService, 
                            IAddressService addressService,
                            IDiscountService discountService,
                            IBraintreeGate braintreeGate)
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
            _productService = productService;
            _addressService = addressService;
            _discountService = discountService;
            _braintreeGate = braintreeGate;
        }


        public List<CartItem> CartItems { get; set; } = new();

        public ApplicationCore.Entities.Order Order { get; set; }

        public ApplicationCore.Entities.Address Address { get; set; }



        public IActionResult OnGet()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Order = _orderService.GetUserCurrentOrder(userId);

            if (Order == null)
            {
                return new NotFoundResult();
            }

            if (Order.AddressId == null)
            {
                return RedirectToPage("Address");
            }

            Order.Order_Product = _orderProductService.GetOrderCurrentProducts(Order.Id).ToList();

            foreach (var item in Order.Order_Product)
            {
                CartItems.Add(new CartItem()
                {
                    Product = _productService.GetProductById(item.ProductId),
                    Quantity = item.Quantity
                });
            }

            Address = _addressService.GetAddress((int)Order.AddressId);

            var gateway = _braintreeGate.GetGeteway();
            var clientToken = gateway.ClientToken.Generate();
            ViewData["ClientToken"] = clientToken;
            ViewData["Total"] = Order.Total;

            return Page();
        }

        public IActionResult OnPost(IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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

            if (result.Target.ProcessorResponseText == "Approved")
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
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
            Address = _addressService.GetAddress((int)Order.AddressId);
        }

        public PartialViewResult OnGetPartialTotal()
        {
            CartItems = new();

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
