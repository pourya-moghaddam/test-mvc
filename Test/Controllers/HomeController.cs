using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, TestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int minPrice, int maxPrice)
        {
            var products = from m in _context.Product
                select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name!.Contains(searchString));
            }
            if (minPrice > 0)
            {
                products = products.Where(p => p.Price > minPrice);
            }
            if (maxPrice > 0)
            {
                products = products.Where(p => p.Price < maxPrice);
            }

            return View(await products.ToListAsync());
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
    }
}
