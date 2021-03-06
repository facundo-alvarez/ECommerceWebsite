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


        public PartialViewResult OnGetAddToCart(int prodId, int quantity = 1)
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
                    order.DiscountCodeId = null;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);

                    return Partial("_CartPartial", order.Order_Product.Count());
                }
                else
                {
                    order = new();
                    order.UserId = userId;
                    order.OrderStatus = "In process";

                    _orderService.InsertOrder(order);
                    _orderService.SaveOrder(order);

                    order = _orderService.GetUserCurrentOrder(userId);

                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);

                    return Partial("_CartPartial", order.Order_Product.Count());
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

                return Partial("_CartPartial", cartItems.Count());
            }


            Product = _productService.GetProductById(prodId);
            Category = _categoryService.GetCategories().Where(c => c.Id == Product.CategoryId).FirstOrDefault();
            RelatedProducts = _productService.GetRelatedProducts(Category).Take(4);
            Tags = Product.Tags.Split(',').ToList();
        }



        public JsonResult OnGetFavorite(int prodId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userProductsFavorites = _favoriteService.GetProductFromUser(userId, prodId);

                if (userProductsFavorites == 0)
                {
                    User_Product up = new User_Product()
                    {
                        UserId = userId,
                        ProductId = prodId,
                    };

                    _favoriteService.AddToFavorite(up);
                    return new JsonResult("Added");
                }
                else
                {
                    _favoriteService.RemoveFromFavorite(userProductsFavorites);
                    return new JsonResult("Removed");
                }
            }

            return new JsonResult("Not Autenticated");
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
