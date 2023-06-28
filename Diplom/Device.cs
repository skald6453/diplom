using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Diplom;

public partial class Device
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }
}
