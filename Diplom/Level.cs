using System;
using System.Collections.Generic;

namespace Diplom;

public partial class Level
{
    public int Level1 { get; set; }

    public int? _5StarSongs { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<ExtraSong> ExtraSongs { get; set; } = new List<ExtraSong>();

    public virtual User? User { get; set; }
}
