using System;
using System.Collections.Generic;

namespace Diplom;

public partial class UsersSong
{
    public int? User { get; set; }

    public string? Song { get; set; }

    public virtual User? UserNavigation { get; set; }
}
