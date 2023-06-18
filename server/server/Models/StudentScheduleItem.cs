using server.ViewModels;

namespace server.Models
{
    public class StudentScheduleItem
    {
        public string? Name { get; set; }

        public List<StudentAlgorithmResultItemViewModel>? Schedule { get; set; }
    }
}
