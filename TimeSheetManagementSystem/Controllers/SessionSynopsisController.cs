using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeSheetManagementSystem.Data;
using TimeSheetManagementSystem.Models;
using TimeSheetManagementSystem.Controllers;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace TimeSheetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    public class SessionSynopsisController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private int? testId { get; set; }

        public SessionSynopsisController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SessionSynopsis
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageIndex)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CreatedBySort"] = sortOrder == "Created" ? "create_desc" : "Created";
            ViewData["UpdatedBySort"] = sortOrder == "Updated" ? "update_desc" : "Updated";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var queryable = from s in _context.SessionSynopses.Include(s => s.CreatedBy).Include(s => s.UpdatedBy)
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                queryable = queryable.Where(s => s.SessionSynopsisName.Contains(searchString)
                                            || s.CreatedBy.FullName.Contains(searchString)
                                            || s.UpdatedBy.FullName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    queryable = queryable.OrderByDescending(s => s.SessionSynopsisName);
                    break;
                case "Created":
                    queryable = queryable.OrderBy(s => s.CreatedBy.FullName);
                    break;
                case "create_desc":
                    queryable = queryable.OrderByDescending(s => s.CreatedBy.FullName);
                    break;
                case "Updated":
                    queryable = queryable.OrderBy(s => s.UpdatedBy.FullName);
                    break;
                case "update_desc":
                    queryable = queryable.OrderByDescending(s => s.UpdatedBy.FullName);
                    break;
                default:
                    queryable = queryable.OrderBy(c => c.SessionSynopsisName);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<SessionSynopsis>.CreateAsync(queryable.AsNoTracking(), pageIndex ?? 1, pageSize));
        }

        // GET: SessionSynopsis/Create
        // To get the view of SessionSynopsis/Create.cshtml
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SessionSynopsis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create", Name = "Create_Session")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionSynopsisName,IsVisible")] SessionSynopsis session)
        {
            //var user = await GetCurrentUserAsync();
            var loginIdName = _userManager.GetUserName(User);
            UserInfo currentUser = await _context.UserInfo
                .Where(userId => userId.LoginUserName == loginIdName)
                .SingleAsync();

            // Create new session 
            SessionSynopsis newSession = new SessionSynopsis();
            newSession.SessionSynopsisName = session.SessionSynopsisName;
            newSession.SessionSynopsisId = session.SessionSynopsisId;
            newSession.UpdatedBy = currentUser;
            newSession.UpdatedById = session.UpdatedById;
            newSession.CreatedBy = currentUser;
            newSession.CreatedById = session.CreatedById;
            newSession.IsVisible = session.IsVisible;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(newSession);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Your session synopsis has been successfully created!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("Update Error", "The session synopsis has already been created.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("Unknown Error", "Some of the field has invalid data.");
                return View();
            }
        }

        // GET: SessionSynopsis/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //testId = (id);

            var sessionSynopsis = await _context.SessionSynopses.SingleOrDefaultAsync(m => m.SessionSynopsisId == id);

            var loginIdName = _userManager.GetUserName(User);
            UserInfo currentUser = await _context.UserInfo
                .Where(userId => userId.LoginUserName == loginIdName)
                .SingleAsync();

            return View(sessionSynopsis);
        }

        // POST: SessionSynopsis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionSynopsisName,IsVisible")] SessionSynopsis session)
        {
            if (ModelState.IsValid)
            {
                // Get the synopsis that is requested to be updated
                SessionSynopsis updateSession = await _context.SessionSynopses.SingleAsync(s => s.SessionSynopsisId == id);

                // Get current user 
                var loginIdName = _userManager.GetUserName(User);
                UserInfo currentUser = await _context.UserInfo
                    .Where(userId => userId.LoginUserName == loginIdName)
                    .SingleAsync();                

                // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-2.1#update-the-edit-page
                // Copy pasta from here!
                if (await TryUpdateModelAsync<SessionSynopsis>(
                        updateSession,
                        "",
                        s => s.SessionSynopsisName, s => s.IsVisible))
                {
                    try
                    {
                        updateSession.UpdatedById = currentUser.UserInfoId;
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Your session synopsis has been updated successfully";
                    }
                    catch (DbUpdateException)
                    {
                        ModelState.AddModelError("Update Error", "The session synopsis has already been created.");
                        return View();
                    }
                } // End of if
                return RedirectToAction("Index");
            } // End of if
            else
            {
                ModelState.AddModelError("Unknown Error", "Some of the field has invalid data.");
                return View();
            }
        }

        // GET: SessionSynopsis/Delete/5
        [HttpGet("Delete/{id}")]
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
        [HttpPost("Delete/{id}"),ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sessionSynopsis = await _context.SessionSynopses.AsNoTracking().SingleOrDefaultAsync(m => m.SessionSynopsisId == id);

            try
            {
                _context.SessionSynopses.Remove(sessionSynopsis);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The Session Synopsis has been deleted successfully";
            }
            catch
            {
                ModelState.AddModelError("Fail", "Failed to delete Session Synopsis");
            }

            return RedirectToAction("Index");
        }

        private bool SessionSynopsisExists(int id)
        {
            return _context.SessionSynopses.Any(e => e.SessionSynopsisId == id);
        }

        // Method to get current logged in user ID with the user manager.  
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        [AcceptVerbs("Get","Post")]
        // With reference to https://www.codeproject.com/Articles/1204076/ASP-NET-Core-MVC-Remote-Validation
        // Edit page also can use cause of https://stackoverflow.com/questions/36122038/asp-net-mvc-remote-validation-logic-on-edit
        public IActionResult Verify([Bind(nameof(SessionSynopsis.SessionSynopsisName),nameof(SessionSynopsis.SessionSynopsisId))]SessionSynopsis sessionSynopsis)
       {
            bool isViewNameValid;
            if (sessionSynopsis.SessionSynopsisId == 0)
            {
                isViewNameValid = !_context.SessionSynopses.Any(x => x.SessionSynopsisName == sessionSynopsis.SessionSynopsisName);
                if (isViewNameValid == false)
                {
                    return Json($"{sessionSynopsis.SessionSynopsisName} is already in use!");
                }
            }
            else
            {
                isViewNameValid = !_context.SessionSynopses.Any(x => x.SessionSynopsisName == sessionSynopsis.SessionSynopsisName && x.SessionSynopsisId 
                != sessionSynopsis.SessionSynopsisId);
            }

            return Json(isViewNameValid);
            //if (sessionSynopsis == null)
            //{
            //    return Json(true);
            //}
        }
    }
}

