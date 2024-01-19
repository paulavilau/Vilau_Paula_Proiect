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
    [Authorize(Policy = "StockEmployees")]
    public class ToysController : Controller
    {
        private readonly ToyStoreContext _context;

        public ToysController(ToyStoreContext context)
        {
            _context = context;
        }

        // GET: Toys
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string sortOrder, 
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CategorySortParam"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["SupplierSortParam"] = sortOrder == "Supplier" ? "supplier_desc" : "Supplier";
            ViewData["RatingSortParam"] = sortOrder == "Rating" ? "rating_desc" : "Rating";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            
            ViewData["CurrentFilter"] = searchString;
            var toys = from t in _context.Toys
                       select t;  
            if(!String.IsNullOrEmpty(searchString))
            {
                toys = toys.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    toys = toys.OrderByDescending(t => t.Name);
                    break;
                case "Price":
                    toys = toys.OrderBy(t => t.Price);
                    break;
                case "price_desc":
                    toys = toys.OrderByDescending(t => t.Price);
                    break;
                case "Category":
                    toys = toys.OrderBy(t => t.CategoryId);
                    break;
                case "category_desc":
                    toys = toys.OrderByDescending(t => t.CategoryId);
                    break;
                case "Supplier":
                    toys = toys.OrderBy(t => t.Supplier.Name);
                    break;
                case "supplier_desc":
                    toys = toys.OrderByDescending(t => t.Supplier.Name);
                    break;
                case "Rating":
                    //toys = toys.OrderBy(t => t.AverageStars);
                    toys = toys.OrderBy(t => t.Reviews.Any() ? t.Reviews.Average(r => r.Stars) : 0);
                    break;
                case "rating_desc":
                    //toys = toys.OrderByDescending(t => t.AverageStars);
                    toys = toys.OrderByDescending(t => t.Reviews.Any() ? t.Reviews.Average(r => r.Stars) : 0);
                    break;
                default:
                    toys = toys.OrderBy(t => t.Name);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Toy>.CreateAsync(toys
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .Include(s => s.Reviews)
                .AsNoTracking()
                ,pageNumber ?? 1, pageSize));
        }

        // GET: Toys/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toy = await _context.Toys
                .Include(s=>s.Supplier)
                .Include(s => s.Reviews)
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(m => m.ToyID == id);
            if (toy == null)
            {
                return NotFound();
            }

            return View(toy);
        }

        // GET: Toys/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            return View();
        }

        // POST: Toys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId,Description,SupplierId,Image,Price")] Toy toy)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(toy);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +"Try again, and if the problem persists ");
            }
            return View(toy);
        }

        // GET: Toys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toy = await _context.Toys.FindAsync(id);
            if (toy == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            return View(toy);
        }

        // POST: Toys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ToyID,Name,CategoryId,Description,SupplierId,Image,Price")] Toy toy)
        {
            if (id != toy.ToyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToyExists(toy.ToyID))
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
            return View(toy);
        }

        // GET: Toys/Delete/5
        public async Task<IActionResult> Delete(int? id,bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toy = await _context.Toys
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ToyID == id);

            if (toy == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again.";
            }

            return View(toy);
        }

        // POST: Toys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toy = await _context.Toys.FindAsync(id);
            if (toy == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Toys.Remove(toy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool ToyExists(int id)
        {
            return _context.Toys.Any(e => e.ToyID == id);
        }
    }
}
