using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class Vote
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    public IdentityUser User { get; set; }

    [Required]
    public int BlogsId { get; set; }

    public bool Is_Upvote { get; set; }


   
}
