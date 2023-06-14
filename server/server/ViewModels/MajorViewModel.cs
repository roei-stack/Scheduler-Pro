using System.ComponentModel.DataAnnotations;

namespace server.ViewModels
{
    public class MajorViewModel
    {
        [Required]
        public string? MajorName { get; set; }

        [Required]
        public List<string>? Bundles { get; set; }
    }
}
