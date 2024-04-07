using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly TestContext _context;

        public CategoriesController(TestContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = _context.Category.Include(c => c.Parent).Include(c => c.Fields);
            return View(await categories.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Fields)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            CreateParentSelectList();
            CreateFieldSelectList();

            var viewModel = new CategoryViewModel
            {
                FieldIds = new List<int>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Category.Fields = new List<CategoryField>();

                if (viewModel.FieldIds != null)
                {
                    foreach (var id in viewModel.FieldIds)
                    {
                        var categoryField = await _context.CategoryField.FindAsync(id);
                        if (categoryField == null)
                        {
                            return NotFound();
                        }

                        viewModel.Category.Fields.Add(categoryField);
                    }
                }
                _context.Add(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CreateParentSelectList();
            CreateFieldSelectList();
            viewModel.FieldIds = new List<int>();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Fields)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            CreateParentSelectList();
            CreateFieldSelectList();

            CategoryViewModel viewModel = new CategoryViewModel
            {
                Category = category,
                FieldIds = new List<int>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (id != viewModel.Category.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                viewModel.Category.Fields = new List<CategoryField>();

                if (viewModel.FieldIds != null)
                {
                    foreach (var fieldId in viewModel.FieldIds)
                    {
                        var categoryField = await _context.CategoryField.FindAsync(fieldId);
                        if (categoryField == null)
                        {
                            return NotFound();
                        }

                        viewModel.Category.Fields.Add(categoryField);
                    }
                }
                _context.Update(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CreateParentSelectList();
            CreateFieldSelectList();
            viewModel.FieldIds = new List<int>();
            
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Fields)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void CreateParentSelectList()
        {
            var parentSelectList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            parentSelectList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["ParentIds"] = parentSelectList;
        }

        private void CreateFieldSelectList()
        {
            var fieldsSelectList = new List<SelectListItem>(new SelectList(_context.CategoryField, "Id", "Name"));
            ViewData["FieldIds"] = fieldsSelectList;
        }
    }
}
