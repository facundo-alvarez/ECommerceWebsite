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
        private readonly IFavoriteService _favoriteService;

        public DetailsModel(
            IProductService productService, ICategoryService categoryService, IFavoriteService favoriteService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _favoriteService = favoriteService;
        }

        public Product Product { get; set; }

        public Category Category { get; set; }

        public IEnumerable<Product> RelatedProducts { get; set; }

        public List<string> Tags { get; set; }
        
        public bool IsAlreadyFavorite { get; set; }

        public Item Item { get; set; }




        public void OnGet(int id)
        {
            Item = new();

            Product = _productService.GetProductById(id);
            Category = _categoryService.GetCategories().Where(c => c.Id == Product.CategoryId).FirstOrDefault();
            RelatedProducts = _productService.GetRelatedProducts(Category).Take(4);
            Tags = Product.Tags.Split(',').ToList();


            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Item = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).FirstOrDefault(p => p.Product.Id == id); ;
            }

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


        public JsonResult OnGetAddToCart(int id, int quantity, int category)
        {

            if (quantity > 20)
                quantity = 20;

            Item prod = new Item();

            Product = _productService.GetProductById(id);

            List<Item> cartItems = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }

            if (cartItems.Any(p => p.Product.Id == id))
            {
                prod = cartItems.FirstOrDefault(p => p.Product.Id == id);
                if(prod.Quantity + quantity > 20)
                {
                    prod.Quantity = 20;
                }
                else
                {
                    prod.Quantity += quantity;
                }
            }
            else
            {
                cartItems.Add(new Item()
                {
                    Product = _productService.GetProductById(id),
                    Quantity = quantity
                });
                prod.Quantity = quantity;
            }

            HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);

            Product = _productService.GetProductById(id);
            Category = _categoryService.GetCategories().Where(c => c.Id == category).FirstOrDefault();
            RelatedProducts = _productService.GetRelatedProducts(Category).Take(4);
            Tags = Product.Tags.Split(',').ToList();

            return new JsonResult(prod.Quantity);

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
            return new PartialViewResult()
            {
                ViewName = "_CartPartial"
            };
        }
    }
}
