using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test1.Models;

public partial class TbAd
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string? Description { get; set; }

    public string ImgName { get; set; } = null!;
    [Required]
    public string? Url { get; set; }

    public int? Hit { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
