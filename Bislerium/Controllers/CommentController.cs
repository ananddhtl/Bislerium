using Bislerium.Data;
using Bislerium.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bislerium.Controllers
{
    public class CommentController : Controller
    {
        private readonly BisleriumContext _context;
        private readonly IWebHostEnvironment _environment;

        public CommentController(BisleriumContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string commentText)
        {
            // Assuming you are using Identity for user authentication
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var comment = new Comment
            {
                UserId = userId,
                comment = commentText,
                BlogId = id
            };

            // Save the comment to the database
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

          
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.UserId != userId)
            {
               
                return Forbid();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> UpdateComment(int id, string commentText)
        {
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.UserId != userId)
            {
               
                return Forbid();
            }

            comment.comment = commentText;
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
