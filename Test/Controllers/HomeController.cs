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

        public async Task<IActionResult> Index(
            string searchString,
            int minPrice,
            int maxPrice,
            int categoryId,
            params Dictionary<int, string[]>[] fieldValues)
        {
            var categories = _context.Category
                .Include(c => c.Fields)
                .ToArray();
            ViewData["Categories"] = categories;
            
            var products = _context.Product
                .Include(p => p.FieldValuePairs)
                .AsQueryable();
            // var productsArray = products.ToArray();
            
            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["FieldValues"] = fieldValues;
            
            if (categoryId > 0)
            {
                var category = _context.Category
                    .Include(c => c.Fields)
                    .FirstOrDefault(c => c.Id == categoryId);
                if (category == null)
                {
                    return NotFound();
                }

                if (category.Fields != null)
                {
                    var categoryFields = category.Fields.ToList();

                    var currentCategory = category;
                    while (currentCategory.ParentId != null)
                    {
                        currentCategory = _context.Category
                            .Include(c => c.Fields)
                            .FirstOrDefault(c => c.Id == currentCategory.ParentId);
                        categoryFields.AddRange(currentCategory?.Fields);
                    }
                    
                    ViewData["CategoryFields"] = categoryFields;
                }

                products = products.Where(p => p.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(searchString))
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
            if (fieldValues.Length > 0)
            {
                IEnumerable<Product> productsList = products.ToList();
                foreach (var fieldValueDict in fieldValues)
                {
                    foreach (var fieldValuePair in fieldValueDict)
                    {
                        productsList = productsList
                            .Where(p => fieldValuePair.Value.Contains(
                                GetFieldValue(p, fieldValuePair.Key)));
                    }
                }

                List<int> productIds = productsList.Select(product => product.Id).ToList();
                products = products.Where(p => productIds.Contains(p.Id));
            }
            return View(await products.ToListAsync());
        }

        private static string? GetFieldValue(Product product, int fieldId)
        {
            var fieldValuePairs = product.FieldValuePairs;
            if (fieldValuePairs != null)
            {
                FieldValuePair? fieldValuePair = fieldValuePairs
                    .FirstOrDefault(p => p.FieldId == fieldId);
                if (fieldValuePair != null)
                {
                    var value = fieldValuePair.Value;
                    return value ?? null;
                }
            }
            return null;
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
