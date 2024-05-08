using Bislerium.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bislerium.Controllers
{
    public class VoteController : Controller
    {
        private readonly BisleriumContext _context;
        private readonly IWebHostEnvironment _environment;
        public VoteController(BisleriumContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> UpVote(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVote = await _context.Vote.FirstOrDefaultAsync(v => v.BlogsId == id && v.UserId == userId);

            if (existingVote == null)
            {
               
                var newVote = new Vote
                {
                    BlogsId = id,
                    UserId = userId,
                    Is_Upvote = true
                };
                _context.Vote.Add(newVote);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Upvoted successfully!" });
            }
            else if (!existingVote.Is_Upvote)
            {
               
                existingVote.Is_Upvote = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Changed vote to upvote successfully!" });
            }
            else
            {
              
                return Json(new { success = false, message = "You have already upvoted this post." });
            }
        }

        public async Task<IActionResult> DownVote(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVote = await _context.Vote.FirstOrDefaultAsync(v => v.BlogsId == id && v.UserId == userId);

            if (existingVote == null)
            {
               
                var newVote = new Vote
                {
                    BlogsId = id,
                    UserId = userId,
                    Is_Upvote = false
                };
                _context.Vote.Add(newVote);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Downvoted successfully!" });
            }
            else if (existingVote.Is_Upvote)
            {
               
                existingVote.Is_Upvote = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Changed vote to downvote successfully!" });
            }
            else
            {
               
                return Json(new { success = false, message = "You have already downvoted this post." });
            }
        }



    }



}

