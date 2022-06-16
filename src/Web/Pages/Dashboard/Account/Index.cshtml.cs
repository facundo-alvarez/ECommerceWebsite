using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Dashboard.Account
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser ApplicationUser { get; set; }


        public async Task OnGet()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser = await _userManager.FindByIdAsync(userId);
        }
    }
}
