using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MVC_Workshop.Models;

namespace MVC_Workshop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the MVCWorkshopUser class
public class MVCWorkshopUser : IdentityUser
{
    public string Role { get; set; }

    [ForeignKey("Student")]
    public int? StudentId { get; set; }

    public Student? Student { get; set; }

    [ForeignKey("Teacher")]
    public int? TeacherId { get; set; }

    public Teacher? Teacher { get; set; }
}

