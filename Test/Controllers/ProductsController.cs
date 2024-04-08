using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TestContext _context;

        public ProductsController(TestContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categoryIdNamePairs = new List<KeyValuePair<int, string>>();
            var products = _context.Product;
            foreach (var product in products)
            {
                var category = await _context.Category
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId);
                if (category != null)
                {
                    var categoryName = category?.Name;
                    var idNamePair = new KeyValuePair<int, string>(product.CategoryId, categoryName);
                    if (!categoryIdNamePairs.Contains(idNamePair))
                    {
                        categoryIdNamePairs.Add(idNamePair);
                    }
                }
            }
            ViewData["CategoryIdNamePairs"] = categoryIdNamePairs;
            
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.FieldValuePairs)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var fieldValuePairs = product.FieldValuePairs;
            var fieldNameValuePairs = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < fieldValuePairs.Count; i++)
            {
                var field = await _context.CategoryField
                    .FirstOrDefaultAsync(f => f.Id == fieldValuePairs[i].FieldId);
                if (field != null)
                {
                    fieldNameValuePairs.Add(new KeyValuePair<string, string>(field?.Name, fieldValuePairs[i].Value));
                }
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            var categoryName = category?.Name;

            ViewData["CategoryName"] = categoryName;
            ViewData["FieldNameValuePairs"] = fieldNameValuePairs;

            return View(product);
        }

        public IActionResult Create(int? categoryId)
        {
            var myList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            myList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["CategoryId"] = myList;
            
            var viewModel = new ProductViewModel
            {
                Product = new Product
                {
                    FieldValuePairs = new List<FieldValuePair>()
                }
            };

            if (categoryId != null)
            {
                ViewData["CurrentCategoryId"] = categoryId;
                
                var category = _context.Category
                    .Include(c => c.Fields)
                    .FirstOrDefault(c => c.Id == categoryId);

                if (category == null)
                {
                    return NotFound();
                }

                ViewData["CategoryName"] = category.Name;

                var categoryFields = GetCategoryFields(categoryId);
                if (categoryFields == null)
                {
                    return NotFound();
                }

                ViewData["CategoryFields"] = categoryFields;

                if (categoryFields != null)
                {
                    for (int i = 0; i < categoryFields.Count; i++)
                    {
                        var fieldValuePair = new FieldValuePair();
                        viewModel.Product.FieldValuePairs.Add(fieldValuePair);
                    }
                }

                SetFieldValues(categoryFields);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Product.Picture = ConvertIFormFileToByteArray(viewModel);

                _context.Add(viewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        private static byte[] ConvertIFormFileToByteArray(ProductViewModel viewModel)
        {
            using var memoryStream = new MemoryStream();
            viewModel.ImageFile.File?.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var myList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            myList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["CategoryId"] = myList;
            
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .Include(p => p.FieldValuePairs)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            
            var viewModel = new ProductViewModel
            {
                Product = product
            };
            viewModel.Product.FieldValuePairs = new List<FieldValuePair>();
            
            ViewData["CurrentCategoryId"] = product.CategoryId;
                
            var category = _context.Category
                .Include(c => c.Fields)
                .FirstOrDefault(c => c.Id == product.CategoryId);

            if (category == null)
            {
                return NotFound();
            }

            ViewData["CategoryName"] = category.Name;

            var categoryFields = GetCategoryFields(product.CategoryId);
            if (categoryFields == null)
            {
                return NotFound();
            }

            ViewData["CategoryFields"] = categoryFields;

            if (categoryFields != null)
            {
                for (int i = 0; i < categoryFields.Count; i++)
                {
                    var fieldValuePair = new FieldValuePair();
                    viewModel.Product.FieldValuePairs.Add(fieldValuePair);
                }
            }

            SetFieldValues(categoryFields);
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (ModelError error in allErrors)
            {
                Console.WriteLine(error.ErrorMessage);
                Console.WriteLine(error.Exception);
            }
            if (id != viewModel.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.Product.Picture = ConvertIFormFileToByteArray(viewModel);
                    var temp = viewModel.Product.FieldValuePairs;
                    viewModel.Product.FieldValuePairs = new List<FieldValuePair>();

                    _context.Update(viewModel.Product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Product.Id))
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

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product
                .Include(p => p.FieldValuePairs)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            foreach (var fieldValuePair in product?.FieldValuePairs)
            {
                _context.FieldValuePair.Remove(fieldValuePair);
            }
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        public static string ConvertByteArrayToImageDataUrl(Product product)
        {
            string imreBase64Data = Convert.ToBase64String(product.Picture);
            return string.Format("data:image/png;base64,{0}", imreBase64Data);
        }

        private List<CategoryField>? GetCategoryFields(int? categoryId)
        {
            var category = _context.Category
                .Include(c => c.Fields)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                return null;
            }
                
            var categoryFields = category.Fields?.ToList();

            var currentCategory = category;
            while (currentCategory?.ParentId != null)
            {
                currentCategory = _context.Category
                    .Include(c => c.Fields)
                    .FirstOrDefault(c => c.Id == currentCategory.ParentId);
                categoryFields?.AddRange(currentCategory.Fields);
            }

            return categoryFields;
        }

        private void SetFieldValues(List<CategoryField>? categoryFields)
        {
            List<SelectListItem> valuesSelectList;
            if (categoryFields != null)
            {
                foreach (var field in categoryFields)
                {
                    valuesSelectList = new List<SelectListItem>(new SelectList(field.Values));
                    ViewData[$"{field.Name}Values"] = valuesSelectList;
                }
            }
        }
    }
}