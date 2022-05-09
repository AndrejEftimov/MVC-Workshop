using MVC_Workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.ViewModels
{
    public class EnrollmentUploadViewModel
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminar File")]
        public IFormFile? SeminarFile { get; set; }
    }
}
