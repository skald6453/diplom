using System;
using System.Collections.Generic;

namespace Diplom;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? Sex { get; set; }

    public string? Timbre { get; set; }

    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();

    public virtual ICollection<UsersRecording> UsersRecordings { get; set; } = new List<UsersRecording>();
}
