using Microsoft.AspNetCore.Identity;

namespace test1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImage { get; set; }
        public string? city {  get; set; }

    }

}
