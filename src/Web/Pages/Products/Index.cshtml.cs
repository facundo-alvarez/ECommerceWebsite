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
        private readonly ICategoryService _categoryService;

        public IndexModel(IPaginationService paginationService, IProductService productService, ICategoryService categoryService)
        {
            _paginationService = paginationService;
            _productService = productService;
            _categoryService = categoryService;
        }

        public IEnumerable<Product> ProductList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 6;


        [BindProperty(SupportsGet = true)]
        public int? MinPrice { get; set; }


        [BindProperty(SupportsGet = true)]
        public int? MaxPrice { get; set; }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }





        public void OnGet()
        {
            var productsFromDb = _productService.GetProducts();
            var categories = _categoryService.GetCategories();


            if(Category != "all")
            {
                var categoryId = categories.Where(c => c.Name.ToLower() == Category).Select(c => c.Id).FirstOrDefault();

                productsFromDb = productsFromDb.Where(p => p.CategoryId == categoryId);
            }


            if (MinPrice != null || MaxPrice != null)
            {
                productsFromDb = productsFromDb.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).OrderBy(p => p.Price).ToList();
            }

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);

        }

        public void OnPost()
        {

            List<Item> cartItems = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }
            if (cartItems.Any(p => p.Product.Id == Product.Id))
            {
                var p = cartItems.FirstOrDefault(p => p.Product.Id == Product.Id);
                int index = cartItems.IndexOf(p);
                cartItems[index].Quantity += 1;
            }
            else
            {
                cartItems.Add(new Item()
                {
                    Product = _productService.GetProductById(Product.Id),
                    Quantity = 1
                });
            }

            HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);


            var productsFromDb = _productService.GetProducts();
            var categories = _categoryService.GetCategories();


            if (Category != "all")
            {
                var categoryId = categories.Where(c => c.Name.ToLower() == Category).Select(c => c.Id).FirstOrDefault();

                productsFromDb = productsFromDb.Where(p => p.CategoryId == categoryId);
            }


            if (MinPrice != null || MaxPrice != null)
            {
                productsFromDb = productsFromDb.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).OrderBy(p => p.Price).ToList();
            }

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);

        }


        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    }
}
