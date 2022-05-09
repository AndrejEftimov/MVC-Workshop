using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.ViewModels
{
    public class EnrollmentsViewModel
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public IList<int> StudentIds { get; set; }
    }
}
