using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BisleriumBloggingWebApp.Models;

namespace BisleriumBloggingWebApp.Controllers
{
    public class ReactionsController : Controller
    {
        private readonly MyDbContext _context;

        public ReactionsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Reactions
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Reactions.Include(r => r.Blog).Include(r => r.ReactionType).Include(r => r.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Reactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reactions
                .Include(r => r.Blog)
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReactionID == id);
            if (reaction == null)
            {
                return NotFound();
            }

            return View(reaction);
        }

        // GET: Reactions/Create
        public IActionResult Create()
        {
            ViewData["BlogID"] = new SelectList(_context.Blogs, "BlogID", "Body");
            ViewData["ReactionTypeID"] = new SelectList(_context.ReactionTypes, "ReactionTypeID", "ReactionName");
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email");
            return View();
        }

        // POST: Reactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReactionID,BlogID,UserID,ReactionTypeID")] Reaction reaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogID"] = new SelectList(_context.Blogs, "BlogID", "Body", reaction.BlogID);
            ViewData["ReactionTypeID"] = new SelectList(_context.ReactionTypes, "ReactionTypeID", "ReactionName", reaction.ReactionTypeID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", reaction.UserID);
            return View(reaction);
        }

        /// My own Create method for Upvotes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoteReaction([Bind("BlogID,UserID,ReactionTypeID")] Reaction newReaction)
        {
            // Get the existing reaction for the current user, blog, and reaction type
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.BlogID == newReaction.BlogID && r.UserID == newReaction.UserID && r.ReactionTypeID == newReaction.ReactionTypeID);

            if (existingReaction == null)
            {
                // Remove any existing reactions for the current user and blog
                var existingReactions = await _context.Reactions
                    .Where(r => r.BlogID == newReaction.BlogID && r.UserID == newReaction.UserID)
                    .ToListAsync();
                _context.Reactions.RemoveRange(existingReactions);

                // Add the new reaction
                _context.Reactions.Add(newReaction);
            }
            else
            {
                // Remove the existing reaction
                _context.Reactions.Remove(existingReaction);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Blogs");
        }
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // GET: Reactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reactions.FindAsync(id);
            if (reaction == null)
            {
                return NotFound();
            }
            ViewData["BlogID"] = new SelectList(_context.Blogs, "BlogID", "Body", reaction.BlogID);
            ViewData["ReactionTypeID"] = new SelectList(_context.ReactionTypes, "ReactionTypeID", "ReactionName", reaction.ReactionTypeID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", reaction.UserID);
            return View(reaction);
        }

        // POST: Reactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReactionID,BlogID,UserID,ReactionTypeID")] Reaction reaction)
        {
            if (id != reaction.ReactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReactionExists(reaction.ReactionID))
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
            ViewData["BlogID"] = new SelectList(_context.Blogs, "BlogID", "Body", reaction.BlogID);
            ViewData["ReactionTypeID"] = new SelectList(_context.ReactionTypes, "ReactionTypeID", "ReactionName", reaction.ReactionTypeID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", reaction.UserID);
            return View(reaction);
        }

        // GET: Reactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _context.Reactions
                .Include(r => r.Blog)
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReactionID == id);
            if (reaction == null)
            {
                return NotFound();
            }

            return View(reaction);
        }

        // POST: Reactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reaction = await _context.Reactions.FindAsync(id);
            if (reaction != null)
            {
                _context.Reactions.Remove(reaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReactionExists(int id)
        {
            return _context.Reactions.Any(e => e.ReactionID == id);
        }
    }
}
