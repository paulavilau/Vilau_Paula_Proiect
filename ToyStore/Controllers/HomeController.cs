using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vilau_Paula_Proiect.Models;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models.ToyStoreViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Vilau_Paula_Proiect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToyStoreContext _context;
        public HomeController(ToyStoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> Statistics()
        {
            IQueryable<SupplierGroup> data =
            from toys in _context.Toys
            group toys by toys.Supplier.Name into supplierGroup
            select new SupplierGroup()
            {
                SupplierName = supplierGroup.Key,
                ToyCount = supplierGroup.Count()
            };
            return View(await data.AsNoTracking().ToListAsync());
        }

        [Authorize]
        public IActionResult Forum()
        {
            return View();
        }
    }
}
