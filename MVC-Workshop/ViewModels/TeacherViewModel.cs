using MVC_Workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.ViewModels
{
    public class TeacherViewModel
    {
        public Teacher Teacher { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }
    }
}
