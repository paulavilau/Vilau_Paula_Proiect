using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vilau_Paula_Proiect.Data;
using Vilau_Paula_Proiect.Models;

namespace Vilau_Paula_Proiect.Controllers
{
    [Authorize(Policy = "StockEmployees")]
    public class SuppliersController : Controller
    {
        private readonly ToyStoreContext _context;
        private string _baseUrl = "http://localhost:5227/api/Suppliers";
        private string _cityUrl = "http://localhost:5227/api/Cities";

        public SuppliersController(ToyStoreContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);

            if (response.IsSuccessStatusCode)
            {
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(await response.Content.
                ReadAsStringAsync());
                return View(suppliers);
            }
            return NotFound();

        }

        // GET: Supplier/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var supplier = JsonConvert.DeserializeObject<Supplier>(
                await response.Content.ReadAsStringAsync());
                return View(supplier);
            }
            return NotFound();
        }
        // GET: Customers/Create
        public async Task<ActionResult> Create()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_cityUrl);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var cities = JsonConvert.DeserializeObject<List<City>>(await response.Content.ReadAsStringAsync());
            ViewData["Cities"] = new SelectList(cities, "CityId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("SupplierID,Name,Address,CityId")] Supplier supplier)
        {
            if (!ModelState.IsValid) return View(supplier);
            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(supplier);
                var response = await client.PostAsync(_baseUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record:{ ex.Message} ");
            }
            return View(supplier);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var client = new HttpClient();
            var cityResponse = await client.GetAsync(_cityUrl);
            if (!cityResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var cities = JsonConvert.DeserializeObject<List<City>>(await cityResponse.Content.ReadAsStringAsync());
            ViewData["Cities"] = new SelectList(cities, "CityId", "Name");

            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var supplier = JsonConvert.DeserializeObject<Supplier>(
                await response.Content.ReadAsStringAsync());
                return View(supplier);
            }
            return new NotFoundResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("SupplierId,Name,Address,CityId")]Supplier supplier)
        {
            if (!ModelState.IsValid) return View(supplier); 
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(supplier);
            var response = await client.PutAsync($"{_baseUrl}/{supplier.SupplierId}",
            new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var supplier = JsonConvert.DeserializeObject<Supplier>(await
               response.Content.ReadAsStringAsync());
                return View(supplier);
            }
            return new NotFoundResult();
        }
        // POST: Customers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind("SupplierId")] Supplier supplier)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Delete,
               $"{_baseUrl}/{supplier.SupplierId}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(supplier),
               Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record:{ ex.Message} ");
            }
            return View(supplier);
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.SupplierId == id);
        }
    }
}

