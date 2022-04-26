using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string? Degree { get; set; }

        [Display(Name = "Academic Rank")]
        [StringLength(25)]
        public string? AcademicRank { get; set; }

        [Display(Name = "Office Number")]
        [StringLength(10)]
        public string? OfficeNumber { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        public ICollection<Course>? CoursesFirst { get; set; }

        public ICollection<Course>? CoursesSecond { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
