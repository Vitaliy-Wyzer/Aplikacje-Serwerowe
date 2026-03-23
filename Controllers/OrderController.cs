using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab1_AS.Models;
using lab1_AS.data;

namespace lab1_AS.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus);
            return View(await orders.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                    .ThenInclude(sh => sh.OrderStatus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            if (order.StatusHistory != null)
            {
                order.StatusHistory = order.StatusHistory.OrderByDescending(sh => sh.ChangedAt).ToList();
            }

            return View(order);
        }

        public async Task<IActionResult> EditStatus(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Id", "Name", order.OrderStatusId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, int orderStatusId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.OrderStatusId != orderStatusId)
            {
                order.OrderStatusId = orderStatusId;

                var history = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    OrderStatusId = orderStatusId,
                    ChangedAt = DateTime.UtcNow
                };

                _context.StatusHistory.Add(history);
                _context.Update(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }
    }
}