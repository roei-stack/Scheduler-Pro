using CourseModel;
using server.Models;
using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class StudentFormViewModel
    {
        [Required]
        public List<Period>? UnavailableTimes { get; set; }

        [Required]
        public List<string>? CourseIds { get; set; }
    }
}
