using MVC_Workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.ViewModels
{
    public class StudentViewModel
    {
        public Student Student { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }
    }
}
