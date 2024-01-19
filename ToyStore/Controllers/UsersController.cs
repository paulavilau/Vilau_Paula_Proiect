using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Vilau_Paula_Proiect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var userToDelete = await userManager.FindByIdAsync(userId);

            if (userToDelete == null)
            {
                return NotFound();
            }

            // Delete the user
            var result = await userManager.DeleteAsync(userToDelete);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users"); 
            }
            else
            {
                return View("Error");
            }
        }

    }
}

