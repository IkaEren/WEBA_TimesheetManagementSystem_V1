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
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            IOrderedQueryable<AccountDetail> test = _context.AccountDetails.Where(a => a.CustomerAccountId == id).OrderBy(x => x.AccountDetailId).Select(acc => new AccountDetail
            {
                IsVisible = acc.IsVisible,
                EffectiveStartDate = acc.EffectiveStartDate,
                EffectiveEndDate = acc.EffectiveEndDate,
                StartTimeInMinutes = acc.StartTimeInMinutes,
                EndTimeInMinutes = acc.EndTimeInMinutes,
                DayOfWeekNumber = acc.DayOfWeekNumber
            }).OrderBy(x => x.DayOfWeekNumber);

            var model = await PagingList<AccountDetail>.CreateAsync(test, 10, page);

            return View(model);
        }
        public IActionResult CreateOneAccountDetailForCustomerAccount()
        {
            return View();
        }
        public IActionResult ManageAccountDetailsForOneCustomerAccount()
        {
            return View();
        }
        public IActionResult UpdateOneAccountDetail()
        {
            return View();
        }

    }
}
