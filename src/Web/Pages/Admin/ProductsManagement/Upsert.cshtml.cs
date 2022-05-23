using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Pages.Admin.ProductsManagement
{
    public class UpsertModel : PageModel
    {
        private readonly IProductService _productSevice;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;

        public UpsertModel(IProductService productSevice, IWebHostEnvironment environment, ICategoryService categoryService)
        {
            _productSevice = productSevice;
            _categoryService = categoryService;
            _environment = environment;
        }
        [BindProperty]
        [Required]
        public Product Product { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }

        public IActionResult OnGet(int? id)
        {
            CategoryDropDown = _categoryService.GetCategories().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            Product = new Product();
            if(id == null)
            {
                return Page();
            }

            Product = _productSevice.GetProductById((int)id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _environment.WebRootPath;

                if(Product.Id == 0)
                {
                    string upload = webRootPath + SiteConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        await files[0].CopyToAsync(fileStream);
                    }

                    Product.Image = fileName + extension;
                    _productSevice.AddProduct(Product); 
                }
                else
                {
                    var oldProduct= _productSevice.GetProductByIdNoTracking(Product.Id);

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + SiteConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, oldProduct.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            await files[0].CopyToAsync(fileStream);
                        }

                        Product.Image = fileName + extension;
                    }
                    else
                    {
                        Product.Image = oldProduct.Image;
                    }
                    _productSevice.UpdateProduct(Product);
                }
                _productSevice.SaveProduct();

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
