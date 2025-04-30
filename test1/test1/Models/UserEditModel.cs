using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class UserEditModel
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter the Email")]

        public string Email { get; set; }

        public string? Number { get; set; }
        public string? City { get; set; }   
    }
}
