using CourseModel;

namespace server.ViewModels
{
    public class StudentInputWithoutInstitution
    {
        public string? Name { get; set; }
        public int LearningDays { get; set; }
        public int HoursPerDay { get; set; }
        public bool SemesterA { get; set; }
        public bool SemesterB { get; set; }
        public bool SemesterSummer { get; set; }
        public List<Period>? UnavilableTimes { get; set; }
        public List<CourseDataViewModel>? CoursesData { get; set; }
    }
}
