using ApplicationCore.Entities;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Web.Utility;

namespace Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Item> Items { get; set; }


        public void OnGet()
        {
            Items = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }
        }


        public void OnGetDelete(int id)
        {
            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                Items.Remove(Items.FirstOrDefault(i => i.Product.Id == id));
                HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Items);
            }
        }

        public void OnGetRemoveQuantity(int id, int quantity)
        {
            Items = new List<Item>();
            int existingQuantity = 0;

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                existingQuantity = Items.FirstOrDefault(i => i.Product.Id == id).Quantity;

                if(existingQuantity != 1)
                {
                    existingQuantity -= 1;
                    Items.FirstOrDefault(i => i.Product.Id == id).Quantity = existingQuantity;
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Items);
                }
            }
        }

        public void OnGetAddQuantity(int id, int quantity)
        {
            Items = new List<Item>();
            int existingQuantity = 0;

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                existingQuantity = Items.FirstOrDefault(i => i.Product.Id == id).Quantity;

                if (existingQuantity != 20)
                {
                    existingQuantity += 1;
                    Items.FirstOrDefault(i => i.Product.Id == id).Quantity = existingQuantity;
                    HttpContext.Session.Set<List<Item>>(SiteConstants.SessionCart, Items);
                }
            }
        }


        public PartialViewResult OnGetPartialCart()
        {
            Items = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }

            return Partial("_CartPartial", Items);
        }

        public PartialViewResult OnGetPartialListCart()
        {
            Items = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                Items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }

            return Partial("_CartListPartial", Items);
        }

    }
}
