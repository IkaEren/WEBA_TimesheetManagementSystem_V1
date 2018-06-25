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
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SessionSynopsisController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionSynopsisController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SessionSynopsis
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SessionSynopses.Include(s => s.CreatedBy).Include(s => s.UpdatedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SessionSynopsis/Details/5
        [HttpGet("Details/{id}")]
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
        // ??? Don't know why is this here but I'll fix it. 
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var loginIdName = _userManager.GetUserName(User);
            UserInfo currentUser = await _context.UserInfo
                .Where(userId => userId.LoginUserName == loginIdName)
                .SingleAsync();

            ViewBag.Email = currentUser.Email;

            //ViewData["CreatedById"] = new SelectList(currentUser, "UserInfoId", "Email");
            //ViewData["UpdatedById"] = new SelectList(currentUser, "UserInfoId", "Email");
            //ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            //ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email");
            return View();
        }

        // POST: SessionSynopsis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create", Name = "Create_Session")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionSynopsisId,SessionSynopsisName,CreatedById,UpdatedById,IsVisible")] SessionSynopsis session)
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

            // TODO: Add if (modelstate.isvalid) here
            try
            {
                _context.Add(newSession);
                await _context.SaveChangesAsync();
                //TempData["Success"] = "Session synopsis successfully created";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                return View(nameof(Create));
            }
            //if (ModelState.IsValid)
            //{
            //    sessionSynopsis.CreatedById = currentUser.UserInfoId;
            //    sessionSynopsis.UpdatedById = currentUser.UserInfoId;
            //    _context.Add(sessionSynopsis);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("Index");
        }
        //ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
        //ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
        //ViewData["CreatedById"] = user;
        //ViewData["UpdatedById"] = user;
        //return View(sessionSynopsis);

        // GET: SessionSynopsis/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var sessionSynopsis = await _context.SessionSynopses.SingleOrDefaultAsync(m => m.SessionSynopsisId == id);
            if (sessionSynopsis == null)
            {
                return NotFound();
            }
            //ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
            //ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
            var loginIdName = _userManager.GetUserName(User);
            UserInfo currentUser = await _context.UserInfo
                .Where(userId => userId.LoginUserName == loginIdName)
                .SingleAsync();

            ViewBag.Email = currentUser.UserInfoId;
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
                    updateSession.UpdatedById = currentUser.UserInfoId;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            else
            {
                return BadRequest();
            }


            // TODO: Add some server side duplication check but for now I guess it's fine for basic functionality. 
            //if (id != sessionSynopsis.SessionSynopsisId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(sessionSynopsis);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!SessionSynopsisExists(sessionSynopsis.SessionSynopsisId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction("Index");
            //}
            //ViewData["CreatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.CreatedById);
            //ViewData["UpdatedById"] = new SelectList(_context.UserInfo, "UserInfoId", "Email", sessionSynopsis.UpdatedById);
            //return View(sessionSynopsis);
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
        [HttpPost("Delete/{id}")]
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

        // Method to get current logged in user ID with the user manager.  
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}

