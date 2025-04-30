using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImage { get; set; }
        public string? city {  get; set; }

        public bool? IsActive { get; set;}
        [Required]
        [StringLength(17)]
        public string FullName {  get; set; }
    }

}
