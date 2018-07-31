using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeSheetManagementSystem.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeSheetManagementSystem.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;
using TimeSheetManagementSystem.Models.AccountDetailViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeSheetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    public class AccountDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        [HttpGet("Index/{id}")]
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            IOrderedQueryable<IndexAccountDetails> test = _context.AccountDetails.Where(a => a.CustomerAccountId == id).OrderBy(x => x.AccountDetailId).Select(acc => new IndexAccountDetails
            {
                IsVisible = acc.IsVisible,
                EffectiveStartDate = acc.EffectiveStartDate,
                EffectiveEndDate = acc.EffectiveEndDate,
                StartTimeInMinutes = ConvertMinToTime(acc.StartTimeInMinutes),
                EndTimeInMinutes = ConvertMinToTime(acc.EndTimeInMinutes),
                DayOfWeek = SetDay(acc.DayOfWeekNumber),
                AccountDetailId = acc.AccountDetailId
            }).OrderBy(x => x.DayOfWeek);

            var model = await PagingList<IndexAccountDetails>.CreateAsync(test, 10, page);

            ViewBag.customerId = id;
            return View(model);
        }

        [HttpGet("Create/{id}")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create/{id}")]
        public async Task<IActionResult> Create(int id, [Bind("DayOfWeekNumber,EffectiveStartDate,EffectiveEndDate,StartTimeInMinutes,EndTimeInMinutes,IsVisible")] AccountDetail createAcc)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            AccountDetail accountDetail = new AccountDetail();
            accountDetail.CustomerAccountId = id;
            accountDetail.StartTimeInMinutes = createAcc.StartTimeInMinutes;
            accountDetail.EndTimeInMinutes = createAcc.EndTimeInMinutes;
            accountDetail.EffectiveStartDate = createAcc.EffectiveStartDate;
            accountDetail.EffectiveEndDate = createAcc.EffectiveEndDate;
            accountDetail.IsVisible = createAcc.IsVisible;
            accountDetail.DayOfWeekNumber = createAcc.DayOfWeekNumber;

            if (await TryUpdateModelAsync<AccountDetail>(accountDetail,"AccountDetail", 
                a => a.CustomerAccountId, a => a.DayOfWeekNumber,
                a => a.EffectiveStartDate, a => a.EffectiveEndDate, 
                a => a.StartTimeInMinutes, a => a.EndTimeInMinutes))
            {
                try
                {
                    _context.AccountDetails.Add(accountDetail);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "The Account Detail has been successfully created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View();
        }

        //[HttpGet("Delete/{id}"]
        //public IActionResult Delete(int id)
        //{
        //    return View();
        //}

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        //[ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var accDetail = await _context.AccountDetails
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.AccountDetailId == id);

            //if (accDetail == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            
            try
            {
                _context.AccountDetails.Remove(accDetail);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The Account Detail has been successfully deleted.";
                //return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Fail", "Failed to delete Account Detail");
            }

            return RedirectToAction(nameof(Index),new { id = accDetail.CustomerAccountId });
        }

        private string SetDay(int i)
        {
            string day;
            switch (i)
            {
                case 1:
                    day = "Monday";
                    return day;
                case 2:
                    day = "Tuesday";
                    return day;
                case 3:
                    day = "Wednesday";
                    return day;
                case 4:
                    day = "Thursday";
                    return day;
                case 5:
                    day = "Friday";
                    return day;
                case 6:
                    day = "Saturday";
                    return day;
                case 7:
                    day = "Sunday";
                    return day;
                default:
                    day = "";
                    return day;
            }
        }

        private string ConvertMinToTime(int i)
        {
            TimeSpan time = TimeSpan.FromMinutes(i);
            string actualTime = time.ToString(@"hh\:mm");
            return actualTime;
        }
        //public IActionResult ManageAccountDetailsForOneCustomerAccount()
        //{
        //    return View();
        //}
        //public IActionResult UpdateOneAccountDetail()
        //{
        //    return View();
        //}

    }
}
