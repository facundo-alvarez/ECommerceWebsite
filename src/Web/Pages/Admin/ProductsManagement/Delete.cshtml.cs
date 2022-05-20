using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Admin.ProductsManagement
{
    public class DeleteModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(IProductService productService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _environment = environment;
        }

        public Product Product { get; set; }

        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
        }

        public RedirectToPageResult OnPost(int id)
        {
            Product = _productService.GetProductById(id);

            string upload = _environment.WebRootPath + SiteConstants.ImagePath;
            var oldFile = Path.Combine(upload, Product.Image);

            if(System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productService.RemoveProduct(Product);
            _productService.SaveProduct();
            return RedirectToPage("Index");
        }
    }
}
