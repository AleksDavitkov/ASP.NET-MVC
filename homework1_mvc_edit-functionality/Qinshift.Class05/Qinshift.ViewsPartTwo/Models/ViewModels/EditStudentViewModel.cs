using System.ComponentModel.DataAnnotations;

namespace Qinshift.ViewsPartTwo.Models.ViewModels
{
    public class EditStudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // Now I can go on here to specify the DateOfBirth to a further specific range
        // like a custom validation attribute
        // but for now I won't over complicate it
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Active Course")]
        public int? ActiveCourseId { get; set; }

        public List<CourseOptionViewModel> Courses { get; set; }
    }
}