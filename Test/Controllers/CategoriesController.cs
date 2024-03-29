using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var testContext = _context.Category.Include(c => c.Parent);
            return View(await testContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            // List<SelectListItem> selectListItemList = new List<SelectListItem>();
            // selectListItemList.Add(
            //     new SelectListItem {
            //         Selected = true, Text = string.Empty, Value = null
            // });
            // var testContext = _context.Category;
            // Category[] categoryArray = testContext.ToArray();
            // foreach (Category category in categoryArray)
            // {
            //     selectListItemList.Add(
            //         new SelectListItem {
            //             Selected = false, Text = category.Name, Value = category.Id.ToString()
            //     });
            // }

            List<SelectListItem> parentSelectList = new List<SelectListItem>(new SelectList(_context.Category, "Id", "Name"));
            parentSelectList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["ParentId"] = parentSelectList;

            List<SelectListItem> fieldsSelectList = new List<SelectListItem>(new SelectList(_context.CategoryField, "Id", "Name"));
            fieldsSelectList.Insert(0, (new SelectListItem { Text = null, Value = null }));
            ViewData["FieldIds"] = fieldsSelectList;

            CategoryViewModel viewModel = new CategoryViewModel();
            // viewModel.Category = new Category();
            viewModel.FieldIds = new List<int>();
            // SelectList selectList = new SelectList(selectListItemList);
            // ViewData["ParentId"] = new SelectList(_context.Category, "Id", "Name");
            // ViewData["ParentId"] = new SelectList(selectListItemList);
            
            return View(viewModel);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,Name,ParentId,Fields")] Category category)
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (ModelError error in allErrors)
            {
                Console.WriteLine(error.ErrorMessage);
                Console.WriteLine(error.Exception);
            }
            if (ModelState.IsValid)
            {
                // var parentCategory = await _context.Category.FindAsync(category.ParentId);
                // parentCategory.Children.Add(category);
                // foreach (var item in _context.Category.ToList())
                // {
                //     Console.WriteLine(item.Name);
                // }

                // if (category.Fields != null)
                // {
                //     category.Fields = ConvertTextInputToJson(category.Fields);
                // }
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                viewModel.Category.Fields = new List<CategoryField>();
                foreach (int id in viewModel.FieldIds)
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
                    viewModel.Category.Fields.Add(categoryField);
                }
                foreach (var field in viewModel.Category.Fields) Console.WriteLine(field.Name);
                _context.Add(viewModel.Category);
                // _context.Update(parentCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ViewData["ParentId"] = new SelectList(_context.Category, "Id", "Id", category.ParentId);
            return View(viewModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.Category, "Id", "Id", category.ParentId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (ModelError error in allErrors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentId"] = new SelectList(_context.Category, "Id", "Id", category.ParentId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
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

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        private string ConvertTextInputToJson(string input)
        {
            var arr = input.Split("\r\n");
            return JsonSerializer.Serialize(arr);
        }
    }
}
