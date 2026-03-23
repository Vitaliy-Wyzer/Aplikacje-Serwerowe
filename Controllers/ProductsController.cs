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

        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categoryId);

            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> ByTag(int tagId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Id == tagId))
                .ToListAsync();

            return View("Index", products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CategoryId")] Product product, int[] selectedTagIds)
        {
            if (ModelState.IsValid)
            {
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            var selectedTagIds = product.Tags?.Select(t => t.Id).ToArray() ?? new int[0];
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTagIds);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId")] Product product, int[] selectedTagIds)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var productToUpdate = await _context.Products
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (productToUpdate == null) return NotFound();

                    productToUpdate.Name = product.Name;
                    productToUpdate.Price = product.Price;
                    productToUpdate.CategoryId = product.CategoryId;

                    productToUpdate.Tags ??= new List<Tag>();
                    productToUpdate.Tags.Clear();

                    if (selectedTagIds != null && selectedTagIds.Length > 0)
                    {
                        var tagsToAdd = await _context.Tags.Where(t => selectedTagIds.Contains(t.Id)).ToListAsync();
                        foreach (var tag in tagsToAdd)
                        {
                            productToUpdate.Tags.Add(tag);
                        }
                    }

                    _context.Update(productToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var tags = await _context.Products.Include(p => p.Tags).Where(p => p.Id == id).SelectMany(p => p.Tags).Select(t => t.Id).ToListAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", tags);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product != null)
            {
                product.Tags?.Clear();
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}