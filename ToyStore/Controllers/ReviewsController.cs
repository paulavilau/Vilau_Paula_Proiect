using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models;

namespace Vilau_Paula_Proiect.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ToyStoreContext _context;

        public ReviewsController(ToyStoreContext context)
        {
            _context = context;
        }


        // GET: Reviews
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> Index()
        {
            var toyStoreContext = _context.Review.Include(r => r.Client).Include(r => r.Toy);
            return View(await toyStoreContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Client)
                .Include(r => r.Toy)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create(int? id)
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name");
            ViewData["ToyId"] = new SelectList(_context.Toys, "ToyID", "Name");
            ViewData["Toy"] = id;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,ClientId,ToyId,Stars,Text")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.ReviewDate = DateTime.Now;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Toys", new { id = review.ToyId });

            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "ClientId", review.ClientId);
            ViewData["ToyId"] = new SelectList(_context.Toys, "ToyID", "ToyID", review.ToyId);
            return RedirectToAction("Details", "Toys", new { id = review.ToyId });
        }

        // GET: Reviews/Edit/5
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "ClientId", review.ClientId);
            ViewData["ToyId"] = new SelectList(_context.Toys, "ToyID", "ToyID", review.ToyId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,ReviewDate,ClientId,ToyId,Stars,Text")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "ClientId", review.ClientId);
            ViewData["ToyId"] = new SelectList(_context.Toys, "ToyID", "ToyID", review.ToyId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Client)
                .Include(r => r.Toy)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again.";
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "StockEmployees")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            try
            {
                _context.Review.Remove(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ReviewId == id);
        }
    }
}
