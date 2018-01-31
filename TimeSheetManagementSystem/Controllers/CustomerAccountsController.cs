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
    public class CustomerAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerAccountsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: CustomerAccounts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CustomerAccounts.Include(c => c.CreatedBy).Include(c => c.UpdatedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CustomerAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerAccount = await _context.CustomerAccounts
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .SingleOrDefaultAsync(m => m.CustomerAccountId == id);
            if (customerAccount == null)
            {
                return NotFound();
            }

            return View(customerAccount);
        }

        // GET: CustomerAccounts/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            return View();
        }

        // POST: CustomerAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerAccountId,AccountName,Comments,IsVisible,CreatedAt,CreatedById,UpdatedAt,UpdatedById")] CustomerAccount customerAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.UpdatedById);
            return View(customerAccount);
        }

        // GET: CustomerAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerAccount = await _context.CustomerAccounts.SingleOrDefaultAsync(m => m.CustomerAccountId == id);
            if (customerAccount == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.UpdatedById);
            return View(customerAccount);
        }

        // POST: CustomerAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerAccountId,AccountName,Comments,IsVisible,CreatedAt,CreatedById,UpdatedAt,UpdatedById")] CustomerAccount customerAccount)
        {
            if (id != customerAccount.CustomerAccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerAccountExists(customerAccount.CustomerAccountId))
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
            ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.CreatedById);
            ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", customerAccount.UpdatedById);
            return View(customerAccount);
        }

        // GET: CustomerAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerAccount = await _context.CustomerAccounts
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .SingleOrDefaultAsync(m => m.CustomerAccountId == id);
            if (customerAccount == null)
            {
                return NotFound();
            }

            return View(customerAccount);
        }

        // POST: CustomerAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerAccount = await _context.CustomerAccounts.SingleOrDefaultAsync(m => m.CustomerAccountId == id);
            _context.CustomerAccounts.Remove(customerAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CustomerAccountExists(int id)
        {
            return _context.CustomerAccounts.Any(e => e.CustomerAccountId == id);
        }
    }
}
