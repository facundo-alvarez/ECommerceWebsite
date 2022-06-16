using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Dashboard.Favorites
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IFavoriteService _favoriteService;

        public IndexModel(IProductService productService, IFavoriteService favoriteService)
        {
            _productService = productService;
            _favoriteService = favoriteService;
        }

        public List<Product> Products { get; set; } = new();

        public void OnGet()
        {
            Products = new List<Product>();

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFavorites = _favoriteService.GetUserProducts(userId).Select(p => p.ProductId);

            foreach (var item in userFavorites)
            {
                Products.Add(_productService.GetProductById(item));
            }
        }
    }
}
