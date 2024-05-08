using Bislerium.Data;
using Bislerium.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bislerium.Controllers
{
    public class BlogController : Controller
    {
        private readonly BisleriumContext _context;
        private readonly IWebHostEnvironment _environment;

        public BlogController(BisleriumContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> AddBlog(Blogs model, IFormFile image)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string uniqueFileName = null;

            try
            {
                if (image != null && image.Length > 0)
                {
                    if (image.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Image", "The image size must be less than or equal to 3 MB.");
                     
                        return Content("<script>alert('The image size must be less than or equal to 3 MB.');</script>", "text/html");
                    }

                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }

                Blogs blog = new Blogs
                {
                    UserId = userId,
                    Image = "/uploads/" + uniqueFileName,
                    Description = model.Description
                };

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file: " + ex.Message);
            }
        }



        public async Task<IActionResult> EditBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return View("~/Views/Frontend/Blog/Edit.cshtml", blog);
        }

        public async Task<IActionResult> UpdateBlog(int id, Blogs updatedBlog, IFormFile newImage)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != updatedBlog.Id)
            {
                return NotFound();
            }

            try
            {
                var existingBlog = await _context.Blogs.FindAsync(id);

                if (existingBlog == null)
                {
                    return NotFound();
                }

                // Check if the user is authorized to update this blog
                if (existingBlog.UserId != userId)
                {
                    return Forbid();
                }

                existingBlog.Description = updatedBlog.Description;

                // Update the image if a new one is provided
                if (newImage != null && newImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await newImage.CopyToAsync(fileStream);
                    }

                    // Delete the existing image file
                    var existingImagePath = Path.Combine(_environment.WebRootPath, existingBlog.Image.TrimStart('/'));
                    if (System.IO.File.Exists(existingImagePath))
                    {
                        System.IO.File.Delete(existingImagePath);
                    }

                    // Update the blog's image path
                    existingBlog.Image = "/uploads/" + uniqueFileName;
                }

                // Save the changes to the database
                _context.Update(existingBlog);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProfilePage", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating blog: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteBlog(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

          
            if (blog.UserId != userId)
            {
                return Forbid();
            }

            try
            {
             
               

    
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting blog: " + ex.Message);
            }
        }


    }
}
