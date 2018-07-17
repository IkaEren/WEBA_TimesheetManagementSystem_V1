using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeSheetManagementSystem.Data;
using TimeSheetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using ReflectionIT.Mvc.Paging;

namespace TimeSheetManagementSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountRatesController : Controller
    {
        // TODO: Touchup on accountrates controller
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRatesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;    
        }
        
        [HttpGet("Index/{id}")]
        // GET: AccountRates
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            //var applicationDbContext = _context.AccountRates.Include(a => a.CustomerAccount);
            ////ViewBag 
            //return View(await applicationDbContext.ToListAsync());

            IOrderedQueryable<AccountRate> testPage = _context.AccountRates.Where(w => w.CustomerAccountId == id).OrderBy(a => a.AccountRateId).Select(accountRate => new AccountRate
            {
                AccountRateId = accountRate.AccountRateId,
                RatePerHour = accountRate.RatePerHour,
                EffectiveStartDate = accountRate.EffectiveStartDate,
                EffectiveEndDate = accountRate.EffectiveEndDate
            }).OrderBy(a => a.AccountRateId);

            var test = await PagingList<AccountRate>.CreateAsync(testPage, 10, page);

            ViewBag.CustomerAccountId = id;
            return View(test);
        }

        // GET: AccountRates/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CustomerAccountId = id;
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
        [HttpGet("Create/{id}")]
        public IActionResult Create(int id)
        {
            ViewBag.CustomerAccountId = id;
            return View();
        }

        // POST: AccountRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("RatePerHour,EffectiveStartDate,EffectiveEndDate")] AccountRate accountRate)
        {
            AccountRate newRate = new AccountRate();

            newRate.CustomerAccountId = id;
            newRate.RatePerHour = accountRate.RatePerHour;
            newRate.EffectiveStartDate = accountRate.EffectiveStartDate;
            newRate.EffectiveEndDate = accountRate.EffectiveEndDate;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.AccountRates.Add(newRate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "The Account Rate has been successfully created.";
                    return RedirectToAction(nameof(Index), new { id = id });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                }
            }
            return View(nameof(Create));
        }

        // GET: AccountRates/Edit/5
        [HttpGet("Edit/{customerId}/{id}")]
        public async Task<IActionResult> Edit(int? id, int customerId )
        {
            //AccountRate accRate = new AccountRate();

            //accRate.RatePerHour = await _context.AccountRates.Where(x => x.AccountRateId == )
            ////if (id == null)
            ////{
            ////    return NotFound();
            ////}

            var accountRate = await _context.AccountRates.SingleOrDefaultAsync(m => m.AccountRateId == id);
            if (accountRate == null)
            {
                return NotFound();
            }
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName", accountRate.CustomerAccountId);
            ViewBag.CustomerAccountId = id;
            return View(accountRate);
        }

        // POST: AccountRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
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
                TempData["Success"] = "Successfully deleted customer account.";
                return RedirectToAction(nameof(Index), new { id = id });
            }
            ViewData["CustomerAccountId"] = new SelectList(_context.CustomerAccounts, "CustomerAccountId", "AccountName", accountRate.CustomerAccountId);
            return View(accountRate);
        }

        // GET: AccountRates/Delete/5
        [HttpGet("Delete/{id}")]
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
            ViewBag.CustomerAccountId = id;
            return View(accountRate);
        }

        // POST: AccountRates/Delete/5
        [HttpPost, ActionName("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountRate = await _context.AccountRates.SingleOrDefaultAsync(m => m.AccountRateId == id);
            _context.AccountRates.Remove(accountRate);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Successfully deleted customer account.";
            return RedirectToAction(nameof(Index), new { id = id });
        }

        private bool AccountRateExists(int id)
        {
            return _context.AccountRates.Any(e => e.AccountRateId == id);
        }
    }
}
