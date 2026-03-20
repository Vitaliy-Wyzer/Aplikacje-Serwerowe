using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab1_AS.Models;
using lab1_AS.data;

namespace lab1_AS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products (Updated for Category Filtering)
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Prepare the dropdown list for the view
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categoryId);

            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Filter if a category was selected in the dropdown
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            return View(await query.ToListAsync());
        }

        // NEW: Action to filter products by a specific tag
        public async Task<IActionResult> ByTag(int tagId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Id == tagId))
                .ToListAsync();

            return View("Index", products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags) // Important for showing tags in details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CategoryId")] Product product, int[] selectedTagIds)
        {
            if (ModelState.IsValid)
            {
                // Attach the selected tags to the product
                if (selectedTagIds != null && selectedTagIds.Length > 0)
                {
                    product.Tags = await _context.Tags.Where(t => selectedTagIds.Contains(t.Id)).ToListAsync();
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(product);
        }

        // ... Edit and Delete methods stay the same as your scaffolded code ...
    }
}