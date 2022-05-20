using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Utility;

namespace Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IPaginationService _paginationService;
        private readonly IProductService _productService;

        public IndexModel(IPaginationService paginationService, IProductService productService)
        {
            _paginationService = paginationService;
            _productService = productService;
        }

        public IEnumerable<Product> ProductList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 6;


        [BindProperty(SupportsGet = true)]
        public int? MinPrice { get; set; } = 1;


        [BindProperty(SupportsGet = true),]
        public int? MaxPrice { get; set; } = 100;



        public void OnGet()
        {
            var productsFromDb = _productService.GetProducts();

            if (MinPrice != 1 || MaxPrice != 100)
            {
                productsFromDb = productsFromDb.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).OrderBy(p => p.Price).ToList();
            }

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);

        }


        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    }
}
