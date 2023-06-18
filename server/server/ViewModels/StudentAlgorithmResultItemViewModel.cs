using CourseModel;
using server.Models;

namespace server.ViewModels
{
    public class StudentAlgorithmResultItemViewModel
    {
        public Period? HourPeriod { get; set; }

        public ScheduledStudentCourseGroupData? GroupData { get; set; }
    }
}
