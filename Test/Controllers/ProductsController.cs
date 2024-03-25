using System.Text.Json;
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
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
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

        public async Task<IActionResult> Create(int? categoryId)
        {
            List<SelectListItem> myList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            myList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["CategoryId"] = myList;
            
            if (categoryId == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            string categoryFieldsString = category.Fields;
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine(categoryFieldsString);
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            string[] categoryFields = JsonSerializer.Deserialize<string[]>(categoryFieldsString);
            ViewBag.CategoryFields = categoryFields;

            // ProductViewModel productViewModel = new ProductViewModel();
            // productViewModel.CategoryFields = new CategoryFieldsModel();
            // productViewModel.CategoryFields.Fields = new Dictionary<string, string>();
            // foreach (string field in categoryFields)
            // {
            //     productViewModel.CategoryFields.Fields.Add(field, null);
            // }
            
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (ModelError error in allErrors)
            {
                Console.WriteLine(error.ErrorMessage);
                Console.WriteLine(error.Exception);
            }
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBB");
            if (ModelState.IsValid)
            {
                viewModel.Product.Picture = ConvertIFormFileToByteArray(viewModel);

                string jsonFields = JsonSerializer.Serialize(viewModel.CategoryFields.Fields);
                Console.WriteLine(jsonFields);
                viewModel.Product.CategoryFields = JsonSerializer.Serialize(viewModel.CategoryFields.Fields);

                _context.Add(viewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        private static byte[] ConvertIFormFileToByteArray(ProductViewModel viewModel)
        {
            using var ms = new MemoryStream();
            viewModel.ImageFile.File?.CopyTo(ms);
            var fileBytes = ms.ToArray();

            return fileBytes;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Product = product
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.Product.Picture = ConvertIFormFileToByteArray(viewModel);

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
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

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

        public IActionResult SubmitCategory()
        {
            List<SelectListItem> myList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            myList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["CategoryId"] = myList;
            return View(new ProductViewModel());
        }
    }
}