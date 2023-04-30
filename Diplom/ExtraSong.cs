using System;
using System.Collections.Generic;

namespace Diplom;

public partial class ExtraSong
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public TimeSpan? Duration { get; set; }

    public byte? Difficulty { get; set; }

    public int? LvlToAchieve { get; set; }

    public virtual Level? LvlToAchieveNavigation { get; set; }
}
