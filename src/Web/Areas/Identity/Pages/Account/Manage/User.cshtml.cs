using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    [BindProperties]
    public class UserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [MaxLength(25)]
        public string? FirstName { get; set; }
        [MaxLength(25)]
        public string? LastName { get; set; }
        public string? Birthdate { get; set; }
        [MaxLength(15)]
        public string? Phone { get; set; }

        public async Task OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            FirstName = user.FirstName;
            LastName = user.LastName;
            Birthdate = user.Birthdate.ToString();
            Phone = user.PhoneNumber;
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (Birthdate != null)
            {
                var dateArray = Birthdate.Split('-');
                DateTime birthDate = new DateTime(Int32.Parse(dateArray[0]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[2]));
                user.Birthdate = birthDate;
            }

            user.PhoneNumber = Phone;
            user.FirstName = this.FirstName;
            user.LastName = this.LastName;

            await _userManager.UpdateAsync(user);

            return RedirectToPage("/Dashboard/Account/Index");
        }
    }
}
