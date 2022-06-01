using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Web.Utility;

namespace Web.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderProductService _orderProductService;
        private readonly IOrderService _orderService;
        private readonly IFavoriteService _favoriteService;

        public DetailsModel(
            IProductService productService, ICategoryService categoryService, IFavoriteService favoriteService, IHttpContextAccessor httpContextAccessor, IOrderProductService orderProductService, IOrderService orderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _orderProductService = orderProductService;
            _orderService = orderService;
            _favoriteService = favoriteService;
        }

        public Product Product { get; set; }

        public Category Category { get; set; }

        public IEnumerable<Product> RelatedProducts { get; set; }

        public List<string> Tags { get; set; }
        
        public bool IsAlreadyFavorite { get; set; }


        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            Category = _categoryService.GetCategories().Where(c => c.Id == Product.CategoryId).FirstOrDefault();
            RelatedProducts = _productService.GetRelatedProducts(Category).Take(4);
            Tags = Product.Tags.Split(',').ToList();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userFavorites = _favoriteService.GetUserProducts(userId).Where(p => p.ProductId == id).FirstOrDefault();
                if(userFavorites != null)
                {
                    IsAlreadyFavorite = true;
                }
            }
        }


        public void OnGetAddToCart(int prodId, int quantity)
        {
            List<Item> cartItems = new();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                cartItems.Add(new Item()
                {
                    ProductId = prodId,
                    Quantity = quantity,
                });
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var order = _orderService.GetUserCurrentOrder(userId);

                if (order != null)
                {
                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;

                    order.HasCupon = false;
                    order.DiscountCode = null;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);
                }
                else
                {
                    order = new();
                    order.UserId = userId;

                    _orderService.InsertOrder(order);
                    _orderService.SaveOrder(order);
                    order.OrderStatus = "In process";

                    order = _orderService.GetUserCurrentOrder(userId);

                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;
                    
                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);
                }
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }

                if (cartItems.Any(p => p.ProductId == prodId))
                {
                    cartItems.FirstOrDefault(p => p.ProductId == prodId).Quantity += quantity;
                }
                else
                {
                    cartItems.Add(new Item()
                    {
                        ProductId = prodId,
                        Quantity = quantity
                    });
                }

                HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);
            }

            Product = _productService.GetProductById(prodId);
            Category = _categoryService.GetCategories().Where(c => c.Id == Product.CategoryId).FirstOrDefault();
            RelatedProducts = _productService.GetRelatedProducts(Category).Take(4);
            Tags = Product.Tags.Split(',').ToList();
        }



        public JsonResult OnGetFavorite(int id, string favorite)
        {
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                var userId =_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                User_Product up = new User_Product()
                {
                    UserId = userId,
                    ProductId = id,
                };

                if(favorite == "False")
                {
                    _favoriteService.AddToFavorite(up);
                    IsAlreadyFavorite = true;
                    return new JsonResult("True");            
                }
                else
                {
                    int favoriteId = _favoriteService.GetId(up);
                    _favoriteService.RemoveFromFavorite(favoriteId);
                    IsAlreadyFavorite = false;
                    return new JsonResult("False");
                }       
            }
            else
            {
                return new JsonResult("Not Autenticated");
            }
        }

        public ActionResult OnGetPartialCart()
        {
            int items = 0;

            if (User.Identity.IsAuthenticated && User.IsInRole(SiteConstants.CustomerRole))
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                items = _orderProductService.GetOrderCurrentProducts(_orderService.GetUserCurrentOrder(userId).Id).Count();

                return Partial("_CartPartial", items);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count();

                    return Partial("_CartPartial", items);
                }
                else
                {
                    return Partial("_CartPartial", items);
                }
            }
        }
    }
}
