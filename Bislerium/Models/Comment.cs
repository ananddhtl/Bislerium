using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;

namespace Bislerium.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public IdentityUser User { get; set; }
        public string UserId { get; set; }
        public string comment { get; set; }
         public int BlogId { get; set; }

        public Blogs Blog { get; set; }


    }
}
