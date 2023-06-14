using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class MultipleUniStaffViewModel
    {
        // list of staff id's
        [Required]
        public List<string>? StaffList { get; set; }

        // list of course id's
        [Required]
        public List<string>? SharedCourses { get; set; }
    }
}
