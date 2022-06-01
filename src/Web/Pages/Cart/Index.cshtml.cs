using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.Utility;

namespace Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;
        private readonly IProductService _productService;

        public IndexModel(IHttpContextAccessor httpContextAccessor, 
                        IOrderService orderService, 
                        IOrderProductService orderProductService,
                        IProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _orderProductService = orderProductService;
            _productService = productService;
        }

        public List<CartItem> CartItems { get; set; }


        public void OnGet()
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);

                if (order != null)
                {
                    var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                    foreach(var product in orderProducts)
                    {
                        Cart.Add(new Item()
                        {
                            ProductId= product.ProductId,
                            Quantity= product.Quantity,
                        });
                    }
                }
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }
            }

            foreach (var item in Cart)
            {
                CartItems.Add(new CartItem()
                {
                    Product = _productService.GetProductById(item.ProductId),
                    Quantity = item.Quantity
                });
            }
        }

        public IActionResult OnPost()
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            { 
                return RedirectToPage("Summary");
            }
            else
            { 
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                    foreach (var item in Cart)
                    {
                        CartItems.Add(new CartItem()
                        {
                            Product = _productService.GetProductById(item.ProductId),
                            Quantity = item.Quantity
                        });
                    }
                }

                return Page();
            }

        }

        public void OnGetDelete(int id)
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);
                _orderProductService.RemoveProductFormCurrentOrder(order.Id, id);

                order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                order.Total = order.SubTotal;

                _orderService.UpdateOrder(order);

                var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id).ToList();

                if(orderProducts.Count() == 0 || orderProducts == null)
                {
                    _orderService.DeleteOrder(order);
                }

                foreach (var product in orderProducts)
                {
                    Cart.Add(new Item()
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                    });
                }
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                    Cart.Remove(Cart.FirstOrDefault(i => i.ProductId == id));
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Cart);
                }
            }
        }

        public void OnGetRemoveQuantity(int id, int quantity)
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var order = _orderService.GetUserCurrentOrder(userId);

                var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                _orderProductService.RemoveProductQuantity(order.Id, id, 1);

                order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                order.Total = order.SubTotal;

                _orderService.UpdateOrder(order);

            }
            else
            { 
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);

                    if (quantity > 1)
                    {
                        Cart.FirstOrDefault(i => i.ProductId == id).Quantity -= 1;
                    }
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Cart);
                }
            }
        }

        public void OnGetAddQuantity(int id, int quantity)
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);
                var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                _orderProductService.AddProductQuantity(order.Id, id, 1);

                order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                order.Total = order.SubTotal;

                _orderService.UpdateOrder(order);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);

                    if (quantity < 100)
                    {
                        Cart.FirstOrDefault(i => i.ProductId == id).Quantity += 1;
                    }
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Cart);
                }
            }
        }

        public PartialViewResult OnGetPartialCart()
        {
            int numberOfItems = 0;

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);

                if(order != null)
                {
                    numberOfItems = _orderProductService.GetOrderCurrentProducts(order.Id).Count();
                }

                return Partial("_CartPartial", numberOfItems);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    numberOfItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count();

                    return Partial("_CartPartial", numberOfItems);
                }

                return Partial("_CartPartial", numberOfItems);
            }
        }

        public PartialViewResult OnGetPartialListCart()
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);

                if (order != null)
                {
                    var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);
                    foreach (var product in orderProducts)
                    {
                        CartItems.Add(new CartItem()
                        {
                            Product = _productService.GetProductById(product.ProductId),
                            Quantity = product.Quantity,
                        });
                    }
                }

                return Partial("_CartListPartial", CartItems);
            }
            else
            { 
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }

                foreach (var item in Cart)
                {
                    CartItems.Add(new CartItem()
                    {
                        Product = _productService.GetProductById(item.ProductId),
                        Quantity = item.Quantity
                    });
                }

                return Partial("_CartListPartial", CartItems);
            }
        }
    }
}
