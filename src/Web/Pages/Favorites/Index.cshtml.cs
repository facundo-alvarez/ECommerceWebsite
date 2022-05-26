using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Favorites
{
    public class IndexModel : PageModel
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;

        public IndexModel(IFavoriteService favoriteService, IHttpContextAccessor httpContextAccessor, IProductService productService)
        {
            _favoriteService = favoriteService;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public List<Product> FavoriteProducts { get; set; } = new();

        public void OnGet()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFavorites = _favoriteService.GetUserProducts(userId).Select(p => p.ProductId);

            foreach (var item in userFavorites)
            {
                var product = _productService.GetProductById(item);
                FavoriteProducts.Add(product);
            }
        }
    }
}
