using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Utility;

namespace Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public List<Item> Items { get; set; }

        [BindProperty]
        public int Id { get; set; }


        public void OnGet()
        {
            Items = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }
        }

        public void OnPost()
        {
            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                Items.Remove(Items.FirstOrDefault(i => i.Product.Id == Id));
                HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Items);
            }
        }
    }
}
