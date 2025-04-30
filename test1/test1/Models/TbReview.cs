using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test1.Models;

public partial class TbReview
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(20,ErrorMessage ="Please enter a name less than 20 letter")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Please enter the number")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "please enter a valid number")]
    public string Number { get; set; }
    [Required]
    [StringLength(200)]
    public string ContentMsg { get; set; } = null!;

    public String? IsPublic { get; set; }
    [Required]
    [Range(0, 5, ErrorMessage = "Enter a valid price")]

    public int Stars { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CarId { get; set; }
  

    public DateTime? UpdatedDate { get; set; }
}
