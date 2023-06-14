using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class CourseViewModel
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int Lecture_points { get; set; }

        [Required]
        public int TA_points { get; set; }

        [Required]
        public Dictionary<string, int>? Lecture_occurrences { get; set; }

        [Required]
        public Dictionary<string, int>? Ta_occurrences { get; set; }

        [Required]
        public int Lecture_parts { get; set; }

        [Required]
        public int TA_after_lecture { get; set; }
    }
}
