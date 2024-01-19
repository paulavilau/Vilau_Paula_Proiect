using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models;
using Vilau_Paula_Proiect.Models.ToyStoreViewModels;

namespace ToyStore.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ToyStoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartsController(ToyStoreContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public async Task<IActionResult> Index(int? id, int? toyID, int? clientId)
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
            .Include(i => i.CartToys)
            .ThenInclude(i => i.Toy)

             .FirstOrDefaultAsync(i => i.UserId == currentUser.Id);

            return View(cart);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(i => i.CartToys)
                .FirstOrDefaultAsync(m => m.CartID == id)
                ;

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {

            return RedirectToAction("Index", "Toys");
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? ToyId, int? Quantity)
        {
            // Verific daca userul curent are un cos de cumparaturi deschis

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            bool hasCarts = await _context.Carts.AnyAsync();
            
            var cart = await _context.Carts
                .Include(i=> i.CartToys)
                .FirstOrDefaultAsync(i => i.UserId == currentUser.Id);

            // Daca nu are, cream unul nou
           if (cart == null)
            {
                cart = new Cart { UserId = currentUser.Id };
                _context.Add(cart);
                //await _context.SaveChangesAsync();
            }

            if (cart.CartToys == null || !cart.CartToys.Any())
            {
                cart.CartToys = new List<CartToy>();
            }

            var existingCartToy = cart.CartToys.FirstOrDefault(ct => ct.ToyID == ToyId);

            if (existingCartToy != null)
            {
                existingCartToy.Quantity += Quantity ?? 1;
            }
            else
            {
                cart.CartToys.Add(new CartToy
                {
                    CartId = cart.CartID,
                    ToyID = ToyId,
                    Quantity = Quantity ?? 1,
                    Value = 0
                });
                
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Carts");
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CartID,CartDate,UserId")] Cart cart)
        {
            if (id != cart.CartID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.CartID == id);

            if (cart == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again.";
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCartToy(int? ToyId, int? CartId)
        {
            var cartToy = await _context.CartToys.FirstOrDefaultAsync(m => m.ToyID == ToyId && m.CartId == CartId);

            if (cartToy != null)
            {
                _context.CartToys.Remove(cartToy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int? id)
        {
            return _context.Carts.Any(e => e.CartID == id);
        }
    }
}
