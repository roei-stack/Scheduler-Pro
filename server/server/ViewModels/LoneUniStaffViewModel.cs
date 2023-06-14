using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class LoneUniStaffViewModel
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public Dictionary<string, Dictionary<string, int>>? CoursesRolesOccurrences { get; set; }
    }
}
