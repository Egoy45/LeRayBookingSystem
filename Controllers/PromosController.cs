using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;

namespace LeRayBookingSystem.Controllers
{
    public class PromosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Promos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Promos.Include(p => p.CreatedByUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Promos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promos = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promos == null)
            {
                return NotFound();
            }

            return View(promos);
        }

        // GET: Promos/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Promos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,Duration,PromoCode,StartDate,EndDate,IsActive,CreatedBy,CreatedAt")] Promos promos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", promos.CreatedBy);
            return View(promos);
        }

        // GET: Promos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promos = await _context.Promos.FindAsync(id);
            if (promos == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", promos.CreatedBy);
            return View(promos);
        }

        // POST: Promos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Duration,PromoCode,StartDate,EndDate,IsActive,CreatedBy,CreatedAt")] Promos promos)
        {
            if (id != promos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromosExists(promos.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", promos.CreatedBy);
            return View(promos);
        }

        // GET: Promos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promos = await _context.Promos
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promos == null)
            {
                return NotFound();
            }

            return View(promos);
        }

        // POST: Promos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promos = await _context.Promos.FindAsync(id);
            if (promos != null)
            {
                _context.Promos.Remove(promos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromosExists(int id)
        {
            return _context.Promos.Any(e => e.Id == id);
        }
    }
}
