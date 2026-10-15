using System;
using System.Collections.Generic;

namespace SchoolApp.Models;

public partial class Teacher
{
    public int Id { get; set; }

    public string Institution { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual User User { get; set; } = null!;
}
