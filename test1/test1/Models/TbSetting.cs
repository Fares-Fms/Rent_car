using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test1.Models;

public partial class TbSetting
{
    public string? SiteName { get; set; }

    public string? Favicon { get; set; }

    public string? Logo { get; set; }
    [Url]
    public string? Facebook { get; set; }
    [Url]
    public string? Instagram { get; set; }

    public string? Whatsapp { get; set; }
    
    public string? HomeImg1 { get; set; }
    
    public string? HomeImg2 { get; set; }

    public string? HomeImg3 { get; set; }

    public string? HomeTxt1 { get; set; }

    public string? HomeTxt2 { get; set; }

    public string? HomeTxt3 { get; set; }

    public string? Description { get; set; }
}
