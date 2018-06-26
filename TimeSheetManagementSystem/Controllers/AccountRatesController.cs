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
    public class AccountRatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountRatesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: AccountRates
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AccountRates.Include(a => a.CustomerAccount);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AccountRates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountRate = await _context.AccountRates
                .Include(a => a.CustomerAccount)
                .SingleOrDefaultAsync(m => m.AccountRateId == id);
            if (accountRate == null)
            {
                return NotFound();
            }

            return View(accountRate);
        }

        // GET: AccountRates/Create
        public IActionResult Create()
        {
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName");
            return View();
        }

        // POST: AccountRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountRateId,CustomerAccountId,RatePerHour,EffectiveStartDate,EffectiveEndDate")] AccountRate accountRate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountRate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName", accountRate.CustomerAccountId);
            return View(accountRate);
        }

        // GET: AccountRates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountRate = await _context.AccountRates.SingleOrDefaultAsync(m => m.AccountRateId == id);
            if (accountRate == null)
            {
                return NotFound();
            }
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName", accountRate.CustomerAccountId);
            return View(accountRate);
        }

        // POST: AccountRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountRateId,CustomerAccountId,RatePerHour,EffectiveStartDate,EffectiveEndDate")] AccountRate accountRate)
        {
            if (id != accountRate.AccountRateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountRateExists(accountRate.AccountRateId))
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
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName", accountRate.CustomerAccountId);
            return View(accountRate);
        }

        // GET: AccountRates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountRate = await _context.AccountRates
                .Include(a => a.CustomerAccount)
                .SingleOrDefaultAsync(m => m.AccountRateId == id);
            if (accountRate == null)
            {
                return NotFound();
            }

            return View(accountRate);
        }

        // POST: AccountRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountRate = await _context.AccountRates.SingleOrDefaultAsync(m => m.AccountRateId == id);
            _context.AccountRates.Remove(accountRate);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AccountRateExists(int id)
        {
            return _context.AccountRates.Any(e => e.AccountRateId == id);
        }
    }
}
