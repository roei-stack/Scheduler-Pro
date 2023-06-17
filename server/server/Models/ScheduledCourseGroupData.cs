using CourseModel;

namespace server.Models
{
    public class ScheduledCourseGroupData
    {
        public List<string>? StaffIds { get; set; }
        public List<Period>? Periods { get; set; }
        public string? Type { get; set; }
    }
}
