using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Display(Name = "Student Id")]
        [StringLength(10)]
        public string StudentId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        [Range(0, 400)]
        [Display(Name = "Acquired Credits")]
        public int? AcquiredCredits { get; set; }

        [Range(1, 16)]
        [Display(Name = "Current Semester")]
        public int? CurrentSemester { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string FullNameWithId
        {
            get { return FirstName + " " + LastName + "  " + StudentId; }
        }
    }
}
