using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BinusZoom.Data;
using BinusZoom.Models;

namespace BinusZoom.Controllers
{
    public class TemporaryLogoController : Controller
    {
        private readonly BinusZoomContext _context;

        public TemporaryLogoController(BinusZoomContext context)
        {
            _context = context;
        }

        // GET: TemporaryLogo
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemporaryLogo.ToListAsync());
        }

        // GET: TemporaryLogo/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporaryLogo = await _context.TemporaryLogo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temporaryLogo == null)
            {
                return NotFound();
            }

            return View(temporaryLogo);
        }

        // GET: TemporaryLogo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemporaryLogo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LogoPath,StartDate,EndDate")] TemporaryLogo temporaryLogo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(temporaryLogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(temporaryLogo);
        }

        // GET: TemporaryLogo/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporaryLogo = await _context.TemporaryLogo.FindAsync(id);
            if (temporaryLogo == null)
            {
                return NotFound();
            }
            return View(temporaryLogo);
        }

        // POST: TemporaryLogo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,LogoPath,StartDate,EndDate")] TemporaryLogo temporaryLogo)
        {
            if (id != temporaryLogo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(temporaryLogo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemporaryLogoExists(temporaryLogo.Id))
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
            return View(temporaryLogo);
        }

        // GET: TemporaryLogo/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var temporaryLogo = await _context.TemporaryLogo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (temporaryLogo == null)
            {
                return NotFound();
            }

            return View(temporaryLogo);
        }

        // POST: TemporaryLogo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var temporaryLogo = await _context.TemporaryLogo.FindAsync(id);
            if (temporaryLogo != null)
            {
                _context.TemporaryLogo.Remove(temporaryLogo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemporaryLogoExists(string id)
        {
            return _context.TemporaryLogo.Any(e => e.Id == id);
        }
    }
}
