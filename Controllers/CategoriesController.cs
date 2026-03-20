using lab1_AS.data;
using lab1_AS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace lab1_AS.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {

            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var categories = _context.Categories.ToList();
            return View("Index", categories);
        }
    }
}