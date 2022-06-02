using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
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

        public List<Product> ProductList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 6;

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }

        public bool IsProductAdded { get; set; }



        public void OnGet()
        {
            Specification<Product> spec = Specification<Product>.All;

            
            var newSpec = new ProductsOnSaleSpecification();

            var productsFromDb = _productService.GetProducts();

            if (Category != "all")
            {
                var categorySpecification = new ProductsByCategorySpecification(Category);
                productsFromDb = _productService.GetProductsWithSpecification(categorySpecification);
            }

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);
        }

        public void OnGetAddToCart(int prodId)
        {
            List<Item> cartItems = new();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                cartItems.Add(new Item()
                {
                    ProductId = prodId,
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
                }
                else
                {
                    order = new();
                    order.UserId = userId;

                    _orderService.InsertOrder(order);
                    _orderService.SaveOrder(order);
                    order.OrderStatus = "In process";

                    order = _orderService.GetUserCurrentOrder(userId);

                    _orderProductService.SetOrderProducts(cartItems, order.Id);

                    order.SubTotal = _orderProductService.GetOrderSubtotal(order.Id);
                    order.Total = order.SubTotal;

                    _orderService.UpdateOrder(order);
                    _orderService.SaveOrder(order);
                }
            }
            else
            {
                if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
                {
                    cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
                }

                if (cartItems.Any(p => p.ProductId == prodId))
                {
                    cartItems.FirstOrDefault(p => p.ProductId == prodId).Quantity += 1;
                }
                else
                {
                    cartItems.Add(new Item()
                    {
                        ProductId = prodId,
                        Quantity = 1
                    });
                }

                HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);
            }
            #region Populate Items
            var productsFromDb = _productService.GetProducts();

            if (Category != "all")
            {
                var categorySpecification = new ProductsByCategorySpecification(Category);
                productsFromDb = _productService.GetProductsWithSpecification(categorySpecification);
            }

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);
            #endregion
        }

        public void OnPost()
        {

            List<Item> cartItems = new List<Item>();

            if (HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart) != null && HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart).Count() > 0)
            {
                cartItems = HttpContext.Session.Get<List<Item>>(SiteConstants.SessionCart);
            }
            if (cartItems.Any(p => p.ProductId == Product.Id))
            {
                cartItems.FirstOrDefault(p => p.ProductId == Product.Id).Quantity += 1;
            }
            else
            {
                cartItems.Add(new Item()
                {
                    ProductId = Product.Id,
                    Quantity = 1
                });
            }

            HttpContext.Session.Set(SiteConstants.SessionCart, cartItems);


            var productsFromDb = _productService.GetProducts();
            var categories = _categoryService.GetCategories();


            if (Category != "all")
            {
                var categorySpecification = new ProductsByCategorySpecification(Category);
                productsFromDb = _productService.GetProductsWithSpecification(categorySpecification);
            }

            IsProductAdded = true;

            Count = productsFromDb.Count();
            ProductList = _paginationService.GetPaginatedResult(productsFromDb, CurrentPage, PageSize);

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


        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    }
}

