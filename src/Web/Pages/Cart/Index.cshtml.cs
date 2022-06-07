using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Authentication;
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

        public PartialViewResult OnGetDelete(int id)
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);
                _orderProductService.RemoveProductFormCurrentOrder(order.Id, id);

                order.DiscountCodeId = null;
                order.HasCupon = false;

                order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                order.Total = order.SubTotal;

                _orderService.UpdateOrder(order);

                var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                if(orderProducts.Count() == 0 || orderProducts == null)
                {
                    _orderService.DeleteOrder(order);
                }

                foreach (var product in orderProducts)
                {
                    CartItems.Add(new CartItem()
                    {
                        Product = _productService.GetProductById(product.ProductId),
                        Quantity = product.Quantity,
                    });
                }
                return Partial("_CartListPartial", CartItems);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                    Cart.Remove(Cart.FirstOrDefault(i => i.ProductId == id));
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Cart);
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

        public PartialViewResult OnGetUpdateQuantity(int id, int updateQuantity)
        {
            List<Item> Cart = new();
            CartItems = new();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var order = _orderService.GetUserCurrentOrder(userId);
                var orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                _orderProductService.UpdateProductQuantity(order.Id, id, updateQuantity);

                order.DiscountCodeId = null;
                order.HasCupon = false;

                order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                order.Total = order.SubTotal;

                _orderService.UpdateOrder(order);

                orderProducts = _orderProductService.GetOrderCurrentProducts(order.Id);

                foreach (var product in orderProducts)
                {
                    CartItems.Add(new CartItem()
                    {
                        Product = _productService.GetProductById(product.ProductId),
                        Quantity = product.Quantity,
                    });
                }

                return Partial("_CartListPartial", CartItems);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    Cart = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);

                    var totalQuantity = Cart.FirstOrDefault(i => i.ProductId == id).Quantity += updateQuantity;

                    if (totalQuantity > 100)
                    {
                        Cart.FirstOrDefault(i => i.ProductId == id).Quantity = 100;
                    }
                    if (totalQuantity < 1)
                    {
                        Cart.FirstOrDefault(i => i.ProductId == id).Quantity = 1;
                    }

                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Cart);
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

    }
}
