using System.ComponentModel.DataAnnotations;

namespace MVC_Workshop.ViewModels
{
    public class UnenrollMultipleViewModel
    {
        public int? CourseId { get; set; }

        public IList<int> StudentIds { get; set; }

        [Required]
        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }
    }
}
