using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test1.Models;

public partial class TbReview
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(20)]
    public string Name { get; set; } = null!;
    [Phone]
    public int Phone { get; set; }
    [Required]
    [StringLength(200)]
    public string ContentMsg { get; set; } = null!;

    public int IsPublic { get; set; }= 0;

    public int Stars { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }
}
