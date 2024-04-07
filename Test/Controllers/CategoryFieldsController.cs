using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class CategoryFieldsController : Controller
    {
        private readonly TestContext _context;

        public CategoryFieldsController(TestContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoryField.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryField = await _context.CategoryField
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryField == null)
            {
                return NotFound();
            }

            return View(categoryField);
        }

        // GET: CategoryFields/Create
        public IActionResult Create()
        {
            // CategoryField categoryField = new CategoryField
            // {
            //     Values = new List<string>()
            // };

            CategoryFieldViewModel viewModel = new CategoryFieldViewModel
            {
                FieldValues = new List<string>()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFieldViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.CategoryField.Values = new List<string>();

                if (viewModel.FieldValues != null)
                {
                    foreach (var value in viewModel.FieldValues)
                    {
                        viewModel.CategoryField.Values.Add(value);
                    }
                }
                _context.Add(viewModel.CategoryField);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.FieldValues = new List<string>();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryField = await _context.CategoryField.FindAsync(id);
            if (categoryField == null)
            {
                return NotFound();
            }

            CategoryFieldViewModel viewModel = new CategoryFieldViewModel
            {
                CategoryField = categoryField,
                FieldValues = new List<string>()
            };

            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFieldViewModel viewModel)
        {
            if (id != viewModel.CategoryField.Id)
            {
                return NotFound();
            }
            
            var categoryField = await _context.CategoryField.FindAsync(id);
            if (categoryField == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (viewModel.FieldValues != null)
                {
                    foreach (var value in viewModel.FieldValues)
                    {
                        categoryField.Values?.Add(value);
                    }
                }
                categoryField.Name = viewModel.CategoryField.Name;
                
                _context.Update(categoryField);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.FieldValues = new List<string>();
            
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoryField = await _context.CategoryField
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryField == null)
            {
                return NotFound();
            }

            return View(categoryField);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryField = await _context.CategoryField.FindAsync(id);
            if (categoryField != null)
            {
                _context.CategoryField.Remove(categoryField);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
