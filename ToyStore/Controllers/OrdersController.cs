using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models;
using Vilau_Paula_Proiect.Models.ToyStoreViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;

namespace Vilau_Paula_Proiect.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ToyStoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ToyStoreContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int? id, int? toyID, int? clientId)
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(User);

            var viewModel = new OrdersIndexData();
            viewModel.Orders = await _context.Orders
            //.Include(i => i.Client)
            //.Include(i => i.User)
            .Include(i => i.OrderedToys)
            .ThenInclude(i => i.Toy)
            .ThenInclude(i => i.Reviews)
            .ThenInclude(i => i.Client)

            .Include(i => i.OrderedToys) // Include the Author for each Book
            .ThenInclude(i => i.Toy.Reviews)

            .Where(i => i.UserId == currentUser.Id)

            .AsNoTracking()
            .OrderByDescending(i => i.OrderDate)
            .ToListAsync();

            if (id != null)
            {
                ViewData["OrderID"] = id.Value;
                Order order = viewModel.Orders.Where(i => i.OrderID == id.Value).Single();
                viewModel.Toys = order.OrderedToys.Select(s => s.Toy);
            }
            if (toyID != null)
            {
                ViewData["ToyID"] = toyID.Value;
                Toy toy = viewModel.Toys.Where(i => i.ToyID == toyID.Value).Single();
                ViewData["ToyName"] = toy.Name;
                viewModel.Reviews = viewModel.Toys.Where(x => x.ToyID == toyID).Single().Reviews;
            }
            return View(viewModel);
        }

        [Authorize(Policy = "CustomerService")]
        public async Task<IActionResult> AllOrders(int? id, int? toyID, int? clientId)
        {

            var viewModel = new OrdersIndexData();
            viewModel.Orders = await _context.Orders
            //.Include(i => i.Client)
            //.Include(i => i.User)
            .Include(i => i.OrderedToys)
            .ThenInclude(i => i.Toy)
            .ThenInclude(i => i.Reviews)
            .ThenInclude(i => i.Client)

            .Include(i => i.OrderedToys) // Include the Author for each Book
            .ThenInclude(i => i.Toy.Reviews)

            .AsNoTracking()
            .OrderBy(i => i.OrderDate)
            .ToListAsync();

            if (id != null)
            {
                ViewData["OrderID"] = id.Value;
                Order order = viewModel.Orders.Where(i => i.OrderID == id.Value).Single();
                viewModel.Toys = order.OrderedToys.Select(s => s.Toy);
            }
            if (toyID != null)
            {
                ViewData["ToyID"] = toyID.Value;
                Toy toy = viewModel.Toys.Where(i => i.ToyID == toyID.Value).Single();
                ViewData["ToyName"] = toy.Name;
                viewModel.Reviews = viewModel.Toys.Where(x => x.ToyID == toyID).Single().Reviews;
            }
            return View("AllOrders",viewModel);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                //.Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create(string selectedToys)
        {
            int[] selectedToysIds = selectedToys?.Split(',').Select(int.Parse).ToArray();
            ViewData["SelectedToys"] = selectedToysIds;
            ViewData["ClientID"] = new SelectList(_context.Clients, "ClientId", "ClientId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, string[] selectedToys)
        {
            if (ModelState.IsValid)
            {
                IdentityUser currentUser = await _userManager.GetUserAsync(User);

                order.OrderDate = DateTime.Now;
                order.UserId = currentUser.Id;
                order.ClientEmail = currentUser.UserName;
                _context.Add(order);
                await _context.SaveChangesAsync();

                if (await TryUpdateModelAsync<Order>(
                order,
                "",
                i => i.OrderDate, i => i.UserId))
                {
                    AddToysToOrder(selectedToys, order);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {

                        ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, ");
                    }
                    var cart = await _context.Carts
                    .Include(c => c.CartToys)
                    .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

                    if (cart != null)
                    {
                        // Step 2: Remove all CartToys associated with the cart
                        _context.CartToys.RemoveRange(cart.CartToys);

                        // Step 3: Delete the cart
                        _context.Carts.Remove(cart);

                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Index));
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            AddToysToOrder(selectedToys, order);

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(i => i.OrderedToys)
                .ThenInclude(i => i.Toy)
                .AsNoTracking()
                .FirstOrDefaultAsync(m=>m.OrderID==id);

            if (order == null)
            {
                return NotFound();
            }

            PopulateOrderedToyData(order);
            //ViewData["ClientID"] = new SelectList(_context.Clients, "ClientId", "ClientId", order.ClientID);
            return View(order);
        }

        private void PopulateOrderedToyData(Order order)
        {
            var allToys = _context.Toys;
            var orderedToys = new HashSet<int?>(order.OrderedToys.Select(c => c.ToyID));
            var viewModel = new List<OrderedToyData>();
            foreach (var toy in allToys)
            {
                viewModel.Add(new OrderedToyData
                {
                    ToyID = toy.ToyID,
                    Name = toy.Name,
                    IsOrdered = orderedToys.Contains(toy.ToyID)
                });
            }
            ViewData["Toys"] = viewModel;
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedToys)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderToUpdate = await _context.Orders
             .Include(i => i.OrderedToys)
             .ThenInclude(i => i.Toy)
             .FirstOrDefaultAsync(m => m.OrderID == id);


            if (await TryUpdateModelAsync<Order>(
                orderToUpdate,
                "",
                i => i.OrderDate, i => i.UserId))
            {
                UpdateOrderedToys(selectedToys, orderToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {

                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, ");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateOrderedToys(selectedToys, orderToUpdate);
            PopulateOrderedToyData(orderToUpdate);
            return View(orderToUpdate);
        }
        private void UpdateOrderedToys(string[] selectedToys, Order orderToUpdate)
        {
            if (selectedToys == null)
            {
                orderToUpdate.OrderedToys = new List<OrderedToy>();
                return;
            }


            var selectedToysHS = new HashSet<string>(selectedToys);
            var orderedToys = new HashSet<int>
            (orderToUpdate.OrderedToys.Select(c => c.Toy.ToyID));
            foreach (var toy in _context.Toys)
            {
                if (selectedToysHS.Contains(toy.ToyID.ToString()))
                {
                    if (!orderedToys.Contains(toy.ToyID))
                    {
                        orderToUpdate.OrderedToys.Add(new OrderedToy
                        {
                            OrderId = orderToUpdate.OrderID, 
                            ToyID = toy.ToyID
                        });
                    }
                }
                else
                {
                    if (orderedToys.Contains(toy.ToyID))
                    {
                        OrderedToy toyToRemove = orderToUpdate.OrderedToys.FirstOrDefault(i
                       => i.ToyID == toy.ToyID);
                        _context.Remove(toyToRemove);
                    }
                }
            }

        }
        private void AddToysToOrder(string[] selectedToys, Order orderToUpdate)
        {
            if (selectedToys == null)
            {
                orderToUpdate.OrderedToys = new List<OrderedToy>();
                return;
            }

            orderToUpdate.OrderedToys = new List<OrderedToy>();

            foreach (var toy in selectedToys)
            {
                orderToUpdate.OrderedToys.Add(new OrderedToy
                {
                    OrderId = orderToUpdate.OrderID,
                    ToyID = Int32.Parse(toy)
                });
                }
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id, string? type, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Type"] = type;

            var order = await _context.Orders
                //.Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again.";
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, string? type)
        {
            var order = await _context.Orders.FindAsync(id);
            string redirectTo;

            if (type=="all")
            {
                redirectTo = "AllOrders";
            }
            else
            {
                redirectTo = "Index";
            }

            if (order == null)
            {
                return RedirectToAction(redirectTo);
            }

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(redirectTo);
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool OrderExists(int? id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
