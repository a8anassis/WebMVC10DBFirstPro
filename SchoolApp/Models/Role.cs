using System;
using System.Collections.Generic;

namespace SchoolApp.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Capability> Capabilities { get; set; } = new List<Capability>();
}
