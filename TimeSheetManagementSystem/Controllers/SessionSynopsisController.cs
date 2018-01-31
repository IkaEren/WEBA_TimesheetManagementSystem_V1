using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeSheetManagementSystem.Data;
using TimeSheetManagementSystem.Models;

namespace TimeSheetManagementSystem.Controllers
{
    public class SessionSynopsisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionSynopsisController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: SessionSynopsis
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SessionSynopses.Include(s => s.CreatedBy).Include(s => s.UpdatedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SessionSynopsis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionSynopsis = await _context.SessionSynopses
                .Include(s => s.CreatedBy)
                .Include(s => s.UpdatedBy)
                .SingleOrDefaultAsync(m => m.SessionSynopsisId == id);
            if (sessionSynopsis == null)
            {
                return NotFound();
            }

            return View(sessionSynopsis);
        }

        // GET: SessionSynopsis/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            return View();
        }

        // POST: SessionSynopsis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionSynopsisId,SessionSynopsisName,CreatedById,UpdatedById,IsVisible")] SessionSynopsis sessionSynopsis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sessionSynopsis);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
            return View(sessionSynopsis);
        }

        // GET: SessionSynopsis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionSynopsis = await _context.SessionSynopses.SingleOrDefaultAsync(m => m.SessionSynopsisId == id);
            if (sessionSynopsis == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
            return View(sessionSynopsis);
        }

        // POST: SessionSynopsis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionSynopsisId,SessionSynopsisName,CreatedById,UpdatedById,IsVisible")] SessionSynopsis sessionSynopsis)
        {
            if (id != sessionSynopsis.SessionSynopsisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessionSynopsis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionSynopsisExists(sessionSynopsis.SessionSynopsisId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
            return View(sessionSynopsis);
        }

        // GET: SessionSynopsis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionSynopsis = await _context.SessionSynopses
                .Include(s => s.CreatedBy)
                .Include(s => s.UpdatedBy)
                .SingleOrDefaultAsync(m => m.SessionSynopsisId == id);
            if (sessionSynopsis == null)
            {
                return NotFound();
            }

            return View(sessionSynopsis);
        }

        // POST: SessionSynopsis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessionSynopsis = await _context.SessionSynopses.SingleOrDefaultAsync(m => m.SessionSynopsisId == id);
            _context.SessionSynopses.Remove(sessionSynopsis);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SessionSynopsisExists(int id)
        {
            return _context.SessionSynopses.Any(e => e.SessionSynopsisId == id);
        }
    }
}
