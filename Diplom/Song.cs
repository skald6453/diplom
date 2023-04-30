using System;
using System.Collections.Generic;

namespace Diplom;

public partial class Song
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Author { get; set; }

    public string? Genre { get; set; }

    public TimeSpan? Duration { get; set; }

    public byte? Difficulty { get; set; }
}
