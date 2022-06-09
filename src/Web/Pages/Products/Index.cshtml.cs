using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using Web.Utility;

namespace Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IPaginationService _paginationService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderProductService _orderProductService;
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(IPaginationService paginationService,
                          IProductService productService,
                          ICategoryService categoryService, 
                          IOrderProductService orderProductService,
                          IOrderService orderService,
                          IHttpContextAccessor httpContextAccessor)
        {
            _paginationService = paginationService;
            _productService = productService;
            _categoryService = categoryService;
            _orderProductService = orderProductService;
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }
        public IReadOnlyList<Product> ProductList { get; set; }

        int CurrentPage = 1;
        int PageSize = 8;


        public void OnGet()
        {
            ProductList = _productService.GetProductsWithSpecification(new ProductsByCategorySpecification(Category));

            HttpContext.Session.SetInt32(SiteConstants.CurrentPage, CurrentPage);
            HttpContext.Session.Remove(SiteConstants.Sorting);
            HttpContext.Session.Remove(SiteConstants.ProductsFilters);

            ProductList = _paginationService.GetPaginatedResult(ProductList, CurrentPage, PageSize);
        }

        public IActionResult OnGetLoadMore()
        {
            List<Filter> filters = new();

            if (HttpContext.Session.Get(SiteConstants.ProductsFilters) != null)
            {
                filters = HttpContext.Session.Get<List<Filter>>(SiteConstants.ProductsFilters);
            }

            if(filters.Count > 0)
            {
                FilterProducts(filters);
            }
            else
            {
                ProductList = _productService.GetProductsWithSpecification(new ProductsByCategorySpecification(Category));
            }

            var sorting = HttpContext.Session.GetString(SiteConstants.Sorting);

            if (!string.IsNullOrEmpty(sorting))
            {
                SortingProducts(sorting);
            }

            if(HttpContext.Session.GetInt32(SiteConstants.CurrentPage) != null)
            {
                CurrentPage = (int)HttpContext.Session.GetInt32(SiteConstants.CurrentPage) + 1;
                HttpContext.Session.SetInt32(SiteConstants.CurrentPage, CurrentPage);
            }

            ProductList = _paginationService.GetPaginatedResult(ProductList, CurrentPage, PageSize);

            if (ProductList.Count() == 0)
            {
                return new JsonResult("empty");
            }


            return Partial("_ProductListPartial", ProductList);
        }

        public PartialViewResult OnGetAddToCart(int id)
        {
            List<Item> cartItems = new();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                cartItems.Add(new Item()
                {
                    ProductId = id,
                    Quantity = 1,
                });
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var order = _orderService.GetUserCurrentOrder(userId);

                if (order != null)
                {
                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;

                    order.HasCupon = false;
                    order.DiscountCodeId = null;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);

                    return Partial("_CartPartial", order.Order_Product.Count());
                }
                else
                {
                    order = new();
                    order.UserId = userId;
                    order.OrderStatus = "In process";

                    _orderService.InsertOrder(order);
                    _orderService.SaveOrder(order);

                    order = _orderService.GetUserCurrentOrder(userId);

                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);

                    return Partial("_CartPartial", order.Order_Product.Count());
                }
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }

                if (cartItems.Any(p => p.ProductId == id))
                {
                    cartItems.FirstOrDefault(p => p.ProductId == id).Quantity += 1;
                }
                else
                {
                    cartItems.Add(new Item()
                    {
                        ProductId = id,
                        Quantity = 1
                    });
                }

                HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);

                return Partial("_CartPartial", cartItems.Count());
            }

            ProductList = _productService.GetProductsWithSpecification(new ProductsByCategorySpecification(Category));
        }

        public ActionResult OnGetPartialCart()
        {
            int items = 0;

            if (User.Identity.IsAuthenticated && User.IsInRole(SiteConstants.CustomerRole))
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                items = _orderProductService.GetOrderCurrentProducts(_orderService.GetUserCurrentOrder(userId).Id).Count();

                return Partial("_CartPartial", items);
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    items = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count();

                    return Partial("_CartPartial", items);
                }
                else
                {
                    return Partial("_CartPartial", items);
                }
            }
        }

        public PartialViewResult OnPostProductsFilters([FromBody] List<Filter> filters)
        {
            HttpContext.Session.Set(SiteConstants.ProductsFilters, filters);

            HttpContext.Session.SetInt32(SiteConstants.CurrentPage, 1);

            FilterProducts(filters);

            var sorting = HttpContext.Session.GetString(SiteConstants.Sorting);

            if(!string.IsNullOrEmpty(sorting))
            {
                SortingProducts(sorting);
            }

            ProductList = _paginationService.GetPaginatedResult(ProductList, CurrentPage, PageSize);


            return Partial("_ProductListPartial", ProductList);
        }

        public PartialViewResult OnGetSort(string id)
        {
            List<Filter> filters = new();

            HttpContext.Session.SetString(SiteConstants.Sorting, id);
            HttpContext.Session.SetInt32(SiteConstants.CurrentPage, 1);

            if (HttpContext.Session.Get(SiteConstants.ProductsFilters) != null)
            {
                filters = HttpContext.Session.Get<List<Filter>>(SiteConstants.ProductsFilters);
            }

            if (filters.Count > 0)
            {
                FilterProducts(filters);
            }
            else
            {
                ProductList = _productService.GetProductsWithSpecification(new ProductsByCategorySpecification(Category));
            }

            SortingProducts(id);

            ProductList = _paginationService.GetPaginatedResult(ProductList, CurrentPage, PageSize);

            return Partial("_ProductListPartial", ProductList);
        }


        //Methods
        void FilterProducts(List<Filter> filters)
        {
            Specification<Product> spec = Specification<Product>.All;

            spec = spec.And(new ProductsByCategorySpecification(Category));

            foreach (var filter in filters)
            {
                if (filter.Id == "onSaleInput")
                    spec = spec.And(new ProductsOnSaleSpecification());
                if (filter.Id == "onStockInput")
                    spec = spec.And(new ProductsOnStockSpecification());
                if (filter.Id == "minPriceInput")
                    spec = spec.And(new ProductsMinPriceSpecification(Convert.ToDecimal(filter.Value)));
                if (filter.Id == "maxPriceInput")
                    spec = spec.And(new ProductsMaxPriceSpecification(Convert.ToDecimal(filter.Value)));
            }

            ProductList = _productService.GetProductsWithSpecification(spec);
        }

        void SortingProducts(string id)
        {
            var sortedProducts = new List<Product>();

            switch (id)
            {
                case "1":
                    ProductList = ProductList.OrderBy(p => p.Price).ToList();
                    break;

                case "2":
                    ProductList = ProductList.OrderByDescending(p => p.Price).ToList();
                    break;
            }
        }
    }
}

