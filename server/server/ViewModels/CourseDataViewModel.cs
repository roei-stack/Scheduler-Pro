using CourseModel;

namespace server.ViewModels
{
    public class CourseDataViewModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public int LectureDuration { get; set; }

        public int ExreciseDuration { get; set; }

        public List<Period>? LectureTimes { get; set; }

        public List<Period>? ExreciseTimes { get; set; }
    }
}
