using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public Course? Course { get; set; }

        public int StudentId { get; set; }

        public Student? Student { get; set; }

        [StringLength(10)]
        public string? Semester { get; set; }

        public int? Year { get; set; }

        public int? Grade { get; set; }

        [Display(Name = "Seminar Url")]
        [StringLength(255)]
        public string? SeminarUrl { get; set; }

        [Display(Name = "Project Url")]
        [StringLength(255)]
        public string? ProjectUrl { get; set; }

        [Display(Name = "Exam Points")]
        public int? ExamPoints { get; set; }

        [Display(Name = "Seminar Points")]
        public int? SeminarPoints { get; set; }

        [Display(Name = "Project Points")]
        public int? ProjectPoints { get; set; }

        [Display(Name = "Additional Points")]
        public int? AdditionalPoints { get; set; }

        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }
    }
}
