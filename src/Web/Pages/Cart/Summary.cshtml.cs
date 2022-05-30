using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Braintree;
using Infrastructure.Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Utility;

namespace Web.Pages.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IBraintreeGate _braintreeGate;

        public SummaryModel(IProductService productService, IBraintreeGate braintreeGate)
        {
            _productService = productService;
            _braintreeGate = braintreeGate;
        }


        public List<Item> Items { get; set; }

        [BindProperty]
        public decimal Total { get; set; }


        public void OnGet()
        {
            Items = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);

                foreach(var item in Items)
                {
                    Total += item.Product.Price * item.Quantity;
                }
            }

            var gateway = _braintreeGate.GetGeteway();
            var clientToken = gateway.ClientToken.Generate();
            ViewData["ClientToken"] = clientToken;
            ViewData["Total"] = Total;
        }

        public IActionResult OnPost(IFormCollection collection)
        {
            string nonceFromTheClient = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = Total,
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

        public void OnGetValidate()
        {

        }
    }
}
