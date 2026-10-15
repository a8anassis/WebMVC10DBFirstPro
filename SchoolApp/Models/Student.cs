using System;
using System.Collections.Generic;

namespace SchoolApp.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Am { get; set; } = null!;

    public string Institution { get; set; } = null!;

    public string? Department { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
