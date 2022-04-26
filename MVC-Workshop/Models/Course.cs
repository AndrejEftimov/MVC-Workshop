using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Workshop.Models
{
    public class Course
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }

        public int Semester { get; set; }

        [StringLength(100)]
        public string? Programme { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }
        
        [Display(Name = "First Teacher")]
        public int? FirstTeacherId { get; set; }
        [Display(Name = "First Teacher")]
        public Teacher? FirstTeacher { get; set; }

        [Display(Name = "Second Teacher")]
        public int? SecondTeacherId { get; set; }
        [Display(Name = "Second Teacher")]
        public Teacher? SecondTeacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
