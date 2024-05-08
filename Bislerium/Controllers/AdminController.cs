using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.Controllers
{
    public class AdminController : Controller
    {

        public async Task<IActionResult> Dashboard()
        {
           

            return View("~/Views/Admin/Index.cshtml");
        }

        public async Task<IActionResult> AddAdminUser()
        {


            return View("~/Views/Admin/AddAdmin.cshtml");
        }
    }
}
