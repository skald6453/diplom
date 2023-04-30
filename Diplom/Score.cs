using System;
using System.Collections.Generic;

namespace Diplom;

public partial class Score
{
    public int UserId { get; set; }

    public int Song { get; set; }

    public int Score1 { get; set; }

    public virtual Song SongNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
