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

        public IndexModel(IPaginationService paginationService)
        {
            _paginationService = paginationService;
        }

        public IEnumerable<Product> ProductList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 6;

        [BindProperty]
        public int MinPriceFilter { get; set; } = 1;

        [BindProperty]
        public int MaxPriceFilter { get; set; } = 100;


        public void OnGet()
        {
            ProductList = _paginationService.GetPaginatedResult(CurrentPage, PageSize);
            Count = _paginationService.GetCount();
        }

        public void OnPostFilter()
        {


        }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    }
}
