using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web.Utility;

namespace Web.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        public DetailsModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public Product Product { get; set; }

        [BindProperty]
        [Required]
        [Range(1, 50)]
        public int Quantity { get; set; } = 1;

        [BindProperty]
        public int ProductId { get; set; }


        public Category Category { get; set; }



        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            Category = _categoryService.GetCategories().Where(c => c.Id == Product.CategoryId).FirstOrDefault();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                List<Item> cartItems = new List<Item>();

                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }
                if(cartItems.Any(p => p.Product.Id == ProductId))
                {
                    var p = cartItems.FirstOrDefault(p => p.Product.Id == ProductId);
                    int index = cartItems.IndexOf(p);
                    cartItems[index].Quantity += this.Quantity;
                }
                else
                {
                    cartItems.Add(new Item()
                    {
                        Product = _productService.GetProductById(ProductId),
                        Quantity = Quantity
                    });
                }

                HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);

                return RedirectToPage("/Products/Index");
            }
            else
            {
                return RedirectToPage("Details");
            }  
        }
    }
}
