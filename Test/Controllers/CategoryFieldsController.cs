using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: CategoryFields
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoryField.ToListAsync());
        }

        // GET: CategoryFields/Details/5
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
            return View();
        }

        // POST: CategoryFields/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CategoryField categoryField)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryField);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryField);
        }

        // GET: CategoryFields/Edit/5
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
            return View(categoryField);
        }

        // POST: CategoryFields/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryField categoryField)
        {
            if (id != categoryField.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryField);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryFieldExists(categoryField.Id))
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
            return View(categoryField);
        }

        // GET: CategoryFields/Delete/5
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

        // POST: CategoryFields/Delete/5
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

        private bool CategoryFieldExists(int id)
        {
            return _context.CategoryField.Any(e => e.Id == id);
        }
    }
}
