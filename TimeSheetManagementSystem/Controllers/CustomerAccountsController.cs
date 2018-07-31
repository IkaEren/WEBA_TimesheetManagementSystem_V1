using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeSheetManagementSystem.Data;
using TimeSheetManagementSystem.Models;
using TimeSheetManagementSystem.ViewModels.CustomerAccountsViewModel;
using Microsoft.AspNetCore.Identity;

namespace TimeSheetManagementSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class CustomerAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerAccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("Index")]
        // GET: CustomerAccounts
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageIndex)
        {
            //Sorting for Name and CreatedAt 
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CreatedDateSort"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var queryable = from c in _context.CustomerAccounts.Include(c => c.CreatedBy).Include(c => c.UpdatedBy)
                            select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                queryable = queryable.Where(c => c.AccountName.Contains(searchString)
                                            || c.CreatedAt.ToString("dd/MM/yyyy").Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    queryable = queryable.OrderByDescending(c => c.AccountName);
                    break;
                //case "name_aesc":
                //    queryable = queryable.OrderBy(c => c.AccountName);
                //    break;
                case "Date":
                    queryable = queryable.OrderBy(c => c.CreatedAt);
                    break;
                case "date_desc":
                    queryable = queryable.OrderByDescending(c => c.CreatedAt);
                    break;
                default:
                    queryable = queryable.OrderBy(c => c.AccountName);
                    break;
            }

            int pageSize = 5;
            //var applicationDbContext = _context.CustomerAccounts.Include(c => c.CreatedBy).Include(c => c.UpdatedBy);
            return View(await PaginatedList<CustomerAccount>.CreateAsync(queryable.AsNoTracking(), pageIndex ?? 1, pageSize));
        }

        [HttpGet("Index/{id}")]
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
        [HttpGet("Create")]
        public IActionResult Create()
        {
            //ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            //ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            return View();
        }

        // POST: CustomerAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountName,Comments,isVisible,EffectiveStartDate,EffectiveEndDate,RatePerHour")] CreateCustomerAccounts customerAccount)
        {
            var loginIdName = _userManager.GetUserName(User);
            UserInfo currentUser = await _context.UserInfo
                .Where(userId => userId.LoginUserName == loginIdName)
                .SingleAsync();

            CustomerAccount newCustomer = new CustomerAccount();
            newCustomer.AccountName = customerAccount.AccountName;
            newCustomer.Comments = customerAccount.Comments;
            newCustomer.IsVisible = customerAccount.isVisible;
            newCustomer.CreatedAt = DateTime.Now;
            newCustomer.CreatedById = currentUser.UserInfoId;
            newCustomer.UpdatedAt = DateTime.Now;
            newCustomer.UpdatedById = currentUser.UserInfoId;

            try
            {
                _context.Add(newCustomer);
                await _context.SaveChangesAsync();

                // Account rates, the code is initialized after the database is updated so that the database can generate the id for the customer accounts.
                // TODO: Implement Account Rates, probably with ViewModel, though I'm not sure if I should create ViewModel for everything. 
                var customerId = await _context.CustomerAccounts.Where(c => c.AccountName == customerAccount.AccountName)
                    .Select(x => x.CustomerAccountId)
                    .SingleAsync();

                AccountRate accRate = new AccountRate();

                accRate.CustomerAccountId = customerId;
                accRate.EffectiveStartDate = customerAccount.EffectiveStartDate.Date;
                accRate.RatePerHour = customerAccount.RatePerHour;

                // TODO: Fix the null checking if possible but it's okay
                // C# 6.0 Monadic null checking
                // Credits - https://damieng.com/blog/2013/12/09/probable-c-6-0-features-illustrated
                // Fix on CA2 rip
                //accRate.EffectiveEndDate = customerAccount?.EffectiveEndDate.Value.Date;

                if (customerAccount.EffectiveEndDate != null)
                {
                    accRate.EffectiveEndDate = customerAccount.EffectiveEndDate.Value.Date;
                }
                else
                {
                    accRate.EffectiveEndDate = null;
                }

                try
                {
                    _context.Add(accRate);
                    TempData["Success"] = "The Customer Account has been successfully created.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                }

                return RedirectToAction("Index");
            }
            catch (DbUpdateException )
            {
                return View(nameof(Create));
            }
        }
        
        // GET: CustomerAccounts/Edit/5
        [HttpGet("Edit/{id}")]
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
            return View(customerAccount);
        }

        // POST: CustomerAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerAccountId,AccountName,Comments,IsVisible,CreatedAt,CreatedById,UpdatedAt,UpdatedById")] CustomerAccount customerAccount)
        {
            if (ModelState.IsValid)
            {
                var updateCustomer = await _context.CustomerAccounts.SingleOrDefaultAsync(s => s.CustomerAccountId == id);

                var loginIdName = _userManager.GetUserName(User);
                UserInfo currentUser = await _context.UserInfo
                    .Where(userId => userId.LoginUserName == loginIdName)
                    .SingleAsync();
                var timeNow = DateTime.Now;

                if (await TryUpdateModelAsync<CustomerAccount>(
                    updateCustomer, 
                    "", 
                    u => u.AccountName, u => u.Comments, u => u.Comments))
                {
                    try
                    {
                        updateCustomer.UpdatedById = currentUser.UserInfoId;
                        updateCustomer.UpdatedAt = timeNow;
                        TempData["Success"] = "The Customer Account has been updated successfully.";    
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // GET: CustomerAccounts/Delete/5
        [HttpGet("Delete/{id}")]
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
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerAccount = await _context.CustomerAccounts.SingleOrDefaultAsync(m => m.CustomerAccountId == id);
            _context.CustomerAccounts.Remove(customerAccount);
            await _context.SaveChangesAsync();
            TempData["Success"] = "The Customer Account has been deleted successfully.";
            return RedirectToAction("Index");
        }

        private bool CustomerAccountExists(int id)
        {
            return _context.CustomerAccounts.Any(e => e.CustomerAccountId == id);
        }
        public async Task<IActionResult> Verify([Bind("AccountName")]CreateCustomerAccounts test)
        {
            CustomerAccount customerAccount = await _context.CustomerAccounts.SingleOrDefaultAsync(
                c => c.AccountName == test.AccountName);

            if (customerAccount == null)
            {
                return Json(true);
            }

            return Json($"{test.AccountName} is already in use!");
        }
    }
}
