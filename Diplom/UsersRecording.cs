using System;
using System.Collections.Generic;

namespace Diplom;

public partial class UsersRecording
{
    public int? User { get; set; }

    public string? File { get; set; }

    public string Name { get; set; } = null!;

    public virtual User? UserNavigation { get; set; }
}
