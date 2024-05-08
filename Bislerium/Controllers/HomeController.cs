using Bislerium.Data;
using Bislerium.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Bislerium.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly BisleriumContext _context;
        public HomeController(ILogger<HomeController> logger, BisleriumContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blogsWithUserInfoAndComments = await _context.Blogs
                .Include(b => b.User) 
                .Include(b => b.Comments) 
                .ToListAsync();

            return View("~/Views/Frontend/Index.cshtml", blogsWithUserInfoAndComments);
        }


        public async Task<IActionResult> ProfilePage()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Error", "Home");
            }

            var userBlogs = await _context.Blogs
                .Include(b => b.User)
                .Where(b => b.UserId == userId) 
                .ToListAsync();

            return View("~/Views/Frontend/Profile.cshtml", userBlogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalBlogs = await _context.Blogs.CountAsync();

            var totalComments = await _context.Comment.CountAsync();

           
            var totalUpvotes = await _context.Vote.CountAsync(vote => vote.Is_Upvote);

           
            var totalDownvotes = await _context.Vote.CountAsync(vote => !vote.Is_Upvote);

            ViewData["TotalBlogs"] = totalBlogs;
            ViewData["TotalComments"] = totalComments;
            ViewData["TotalUpvotes"] = totalUpvotes;
            ViewData["TotalDownvotes"] = totalDownvotes;

            return View("~/Views/Admin/Index.cshtml");
        }



        public async Task<IActionResult> AddAdminUser()
        {


            return View("~/Views/Admin/AddAdmin.cshtml");
        }

    }
}
