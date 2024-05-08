using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.Models
{

    public class Blogs
    {
        public int Id { get; set; }
        public  IdentityUser User { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [StringLength(90)]
        public string Image { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(90)]
        public string Description { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }


}
