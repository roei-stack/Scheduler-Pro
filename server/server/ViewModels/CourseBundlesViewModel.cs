using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class CourseBundlesViewModel
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public int MinCreditPoints { get; set; }

        [Required]
        public int MaxCreditPoints { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public List<string>? Courses { get; set; }
    }
}
