using Microsoft.AspNetCore.Identity;

namespace ForumAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        //public string? Image { get; set; }

        public DateTime JoindeDate { get; set; }
    }
}
